using UnityEngine;

public class Bit : MonoBehaviour // every index is ordered from right to left 
{
	// read single byte from four-bytes unsigned int number, index must have values from 0 to 3
	uint ReadByteFromUint(uint u32, int index)
	{
		return (u32 >> (index << 3)) & 255u;
	}

	// read single bit from single byte, index must have values from 0 to 7
	uint ReadBitFromByte(uint bajt, int index)
	{
		return (bajt >> index) & 1u;
	}

	// write single bit (0 or 1) to single byte, index must have values from 0 to 7
	uint WriteBitToByte (uint bit, uint bajt, int index)
	{
		return (bajt & ~(1u << index)) | (bit << index);
	}

	// write single byte to four-bytes unsigned int number, index must have values from 0 to 3
	uint WriteByteToUint(uint bajt, uint u32, int index)
	{
		return (bajt << (index << 3)) | (u32 & (4294967295u ^ (255u << (index << 3))));
	}

	// returns the count of set bits (value of 1) in a 32-bit uint
	uint BitCount(uint i)
	{
		i = i - ((i >> 1) & 0x55555555u);
		i = (i & 0x33333333u) + ((i >> 2) & 0x33333333u);
		return (((i + (i >> 4)) & 0x0F0F0F0Fu) * 0x01010101u) >> 24;
	}

	void Start()
	{
		Debug.Log(WriteByteToUint(WriteBitToByte (1u, 0u, 5), 0u, 3)); // 536870912 (dec) = 0010 0000 0000 0000 0000 0000 0000 0000 (bin)
	}
}