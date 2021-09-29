using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

public class DisassemblerTest : MonoBehaviour
{
	void Start()
	{
		Type type = Type.GetType("Program,Assembly-CSharp");
		MethodInfo methodInfo = type.GetMethod ("AngleVectorPlane", BindingFlags.Instance | BindingFlags.NonPublic);
		Debug.Log (Disassembler.Disassemble(methodInfo));
	}
}

public class Program
{
	float AngleVectorPlane(Vector3 vector, Vector3 normal)
	{
		float number = (float)Math.Acos((double)Vector3.Dot(vector, normal));
		return 1.5707964f - number;
	}
}

// http://www.albahari.com/nutshell/E9-CH18.aspx
public class Disassembler
{
	public static string Disassemble (MethodBase method) => new Disassembler (method).Result();

	static Dictionary<short, OpCode> _OpCodes = new Dictionary<short, OpCode>();

	static Disassembler()
	{
		Dictionary<short, OpCode> opcodes = new Dictionary<short, OpCode>();
		foreach (FieldInfo info in typeof (OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
		if (typeof (OpCode).IsAssignableFrom (info.FieldType))
		{
			OpCode code = (OpCode)info.GetValue (null);
			if (code.OpCodeType != OpCodeType.Nternal) _OpCodes.Add (code.Value, code);
		}
	}

	StringBuilder _Builder;
	Module _Module;
	byte[] _Bytes;
	int _Position;

	Disassembler (MethodBase method)
	{
		_Module = method.DeclaringType.Module;
		_Bytes = method.GetMethodBody().GetILAsByteArray();
	}

	string Result()
	{
		_Builder = new StringBuilder();
		while (_Position < _Bytes.Length) DisassembleNextInstruction();
		return _Builder.ToString();
	}

	void DisassembleNextInstruction()
	{
		int opStart = _Position;
		OpCode code = ReadOpCode();
		string operand = ReadOperand (code);
		_Builder.AppendFormat ("IL_{0:X4}:  {1,-12} {2}", opStart, code.Name, operand);
		_Builder.AppendLine();
	}

	OpCode ReadOpCode()
	{
		byte byteCode = _Bytes [_Position++];
		if (_OpCodes.ContainsKey (byteCode)) return _OpCodes [byteCode];
		if (_Position == _Bytes.Length) throw new Exception ("Unexpected end of IL");
		short shortCode = (short)(byteCode * 256 + _Bytes [_Position++]);
		if (!_OpCodes.ContainsKey (shortCode)) throw new Exception ("Cannot find opcode " + shortCode);
		return _OpCodes [shortCode];
	}

	string ReadOperand (OpCode c)
	{
		int operandLength =
			c.OperandType == OperandType.InlineNone ? 0 :
			c.OperandType == OperandType.ShortInlineBrTarget ||
			c.OperandType == OperandType.ShortInlineI ||
			c.OperandType == OperandType.ShortInlineVar ? 1 :
			c.OperandType == OperandType.InlineVar ? 2 :
			c.OperandType == OperandType.InlineI8 ||
			c.OperandType == OperandType.InlineR ? 8 :
			c.OperandType == OperandType.InlineSwitch ? 4 * (BitConverter.ToInt32 (_Bytes, _Position) + 1) : 4;
		if (_Position + operandLength > _Bytes.Length) throw new Exception ("Unexpected end of IL");
		string result = FormatOperand (c, operandLength);
		if (result == null)
		{
			result = "";
			for (int i = 0; i < operandLength; i++) result += _Bytes [_Position + i].ToString ("X2") + " ";
		}
		_Position += operandLength;
		return result;
	}

	string FormatOperand (OpCode c, int operandLength)
	{
		if (operandLength == 0) return "";
		if (operandLength == 4) return Get4ByteOperand (c);
		else if (c.OperandType == OperandType.ShortInlineBrTarget) return GetShortRelativeTarget();
		else if (c.OperandType == OperandType.InlineSwitch) return GetSwitchTarget (operandLength);
		else return null;
	}

	string Get4ByteOperand (OpCode c)
	{
		int intOp = BitConverter.ToInt32 (_Bytes, _Position);
		switch (c.OperandType)
		{
			case OperandType.InlineTok:
			case OperandType.InlineMethod:
			case OperandType.InlineField:
			case OperandType.InlineType:
				MemberInfo mi;
				try { mi = _Module.ResolveMember (intOp); }
				catch { return null; }
				if (mi == null) return null;
				if (mi.ReflectedType != null) return mi.ReflectedType.FullName + "." + mi.Name;
				else if (mi is Type) return ((Type)mi).FullName;
				else return mi.Name;
			case OperandType.InlineString:
				string s = _Module.ResolveString (intOp);
				if (s != null) s = "'" + s + "'";
				return s;
			case OperandType.InlineBrTarget:
				return "IL_" + (_Position + intOp + 4).ToString ("X4");
			default:
				return null;
		}
	}

	string GetShortRelativeTarget()
	{
		int absoluteTarget = _Position + (sbyte)_Bytes [_Position] + 1;
		return "IL_" + absoluteTarget.ToString ("X4");
	}

	string GetSwitchTarget (int operandLength)
	{
		int targetCount = BitConverter.ToInt32 (_Bytes, _Position);
		string [] targets = new string [targetCount];
		for (int i = 0; i < targetCount; i++)
		{
			int ilTarget = BitConverter.ToInt32 (_Bytes, _Position + (i + 1) * 4);
			targets [i] = "IL_" + (_Position + ilTarget + operandLength).ToString ("X4");
		}
		return "(" + string.Join (", ", targets) + ")";
	}
}