// Author: Przemyslaw Zaworski
// Reference: https://gml.noaa.gov/grad/solcalc/calcdetails.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SunPositionCalculator : MonoBehaviour
{
	[Header("Initial Settings")]
	[Range(0, 23)] public int Hour = 12;
	[Range(0, 59)] public int Minute = 30;
	[Range(0, 59)] public int Second = 0;
	[Range(1, 31)] public int Day = 7;
	[Range(1, 12)] public int Month = 11;
	[Range(1901, 2099)] public int Year = 2023;
	[Range(-90f, 90f)] public double Latitude = 51.759445f;
	[Range(-180f, 180f)] public double Longitude = 19.457216f;
	[Range(-12, 12)] public int Timezone = 1;
	public Light SunLight;
	public bool ShowInfo = true;
	public KeyCode KeyDay = KeyCode.U;
	public KeyCode KeyHour = KeyCode.I;
	public KeyCode KeyMinute = KeyCode.O;
	public KeyCode KeySecond = KeyCode.P;

	private DateTimeOffset _DateTimeOffset;
	private GUIStyle _GUIStyle = new GUIStyle();
	private Transform _Transform;

	void Start()
	{
		if (SunLight == null) SunLight = FindObjectOfType<Light>();
		_Transform = SunLight.transform;
		_GUIStyle.fontSize = 64;
		_GUIStyle.normal.textColor = Color.white;
		_DateTimeOffset = new DateTimeOffset(Year, Month, Day, Hour, Minute, Second, TimeSpan.FromHours(Timezone));
	}

	void Update()
	{
		ChangeTime();
		SunRotation();
	}

	void OnGUI()
	{
		if (ShowInfo)
		{
			DateTime dt = _DateTimeOffset.DateTime;
			string time = dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2") + ":" + dt.Second.ToString("D2");
			string date = dt.Day.ToString("D2") + "." + dt.Month.ToString("D2") + "." + dt.Year.ToString();
			GUI.Label(new Rect(0, 0, 400, 100), time, _GUIStyle);
			GUI.Label(new Rect(0, 100, 400, 100), date, _GUIStyle);
			GUI.Label(new Rect(0, 200, 400, 100), _Transform.eulerAngles.ToString(), _GUIStyle);
		}
	}

	void OnValidate()
	{
		_DateTimeOffset = new DateTimeOffset(Year, Month, Day, Hour, Minute, Second, TimeSpan.FromHours(Timezone));
	}

	void SetTime(TimeSpan timeSpan)
	{
		DateTime dateTime = _DateTimeOffset.DateTime + timeSpan;
		_DateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.FromHours(Timezone));
	}

	void ChangeTime()
	{
		if (Input.GetKeyDown(KeyDay))  SetTime(new TimeSpan(1, 0, 0, 0));
		if (Input.GetKeyDown(KeyHour)) SetTime(new TimeSpan(0, 1, 0, 0));
		if (Input.GetKey(KeyMinute))   SetTime(new TimeSpan(0, 0, 1, 0));
		if (Input.GetKey(KeySecond))   SetTime(new TimeSpan(0, 0, 0, 1));
	}

	Vector2 SunPosition()
	{
		float verticalAngle = (float) SolarElevation();
		float horizontalAngle = 180.0f + (float) SolarAzimuth(); // add 180 because Unity calcs angle from south (-Z)
		return new Vector2(verticalAngle, horizontalAngle);
	}

	void SunRotation()
	{
		Vector2 vector = SunPosition();
		_Transform.rotation = Quaternion.Euler(vector.x, vector.y, 0);
	}

	double Degrees(double radians)
	{
		return radians * 57.2957795131;
	}

	double Radians(double degrees)
	{
		return degrees * 0.01745329251;
	}

	double Reduce(double angle)
	{
		return angle - (Math.Floor(angle / 360.0) * 360.0);
	}

	double Mod(double x, double y)
	{
		return x - y * Math.Floor(x / y);
	}

	double TimePastLocalMidnight()
	{
		DateTime dateTime = new DateTime(1899, 12, 30, 0, 0, 0).Add(_DateTimeOffset.TimeOfDay);
		return (double)dateTime.Subtract(new DateTime(1899, 12, 30).Date).TotalDays;
	}

	double JulianDay()
	{
		double timeZoneOffset = (double) _DateTimeOffset.Offset.TotalHours;
		double date = (double)_DateTimeOffset.Date.Subtract(new DateTime(1899, 12, 30).Date).TotalDays;
		double result = (_DateTimeOffset.Date.Date <= new DateTime(1900, 1, 1)) ? 1.0 + (date - Math.Floor(date)) : date;
		return result + 2415018.5 - (timeZoneOffset / 24.0);
	}

	double JulianCentury()
	{
		return (JulianDay() - 2451545.0) / 36525.0;
	}

	double SunGeometricMeanLongitude()
	{
		double century = JulianCentury();
		return Mod(280.46646 + century * (36000.76983 + century * 0.0003032), 360.0);
	}

	double SunMeanAnomaly()
	{
		double century = JulianCentury();
		return 357.52911 + century * (35999.05029 - 0.0001537 * century);
	}

	double EccentricityOfEarthOrbit()
	{
		double century = JulianCentury();
		return 0.016708634 - century * (0.000042037 + 0.0000001267 * century);
	}

	double SunEquationOfCenter()
	{
		double century = JulianCentury();
		double mean = SunMeanAnomaly();
		double a = Math.Sin(Radians(mean));
		double b = 1.914602 - century * (0.004817 + 0.000014 * century);
		double c = Math.Sin(Radians(mean * 2.0)) * (0.019993 - 0.000101 * century);
		double d = Math.Sin(Radians(mean * 3.0)) * 0.000289;
		return a * b + c + d;
	}

	double SunTrueLongitude()
	{
		return SunGeometricMeanLongitude() + SunEquationOfCenter();
	}

	double SunApparentLongitude()
	{
		double x = 125.04 - 1934.136 * JulianCentury();
		return SunTrueLongitude() - 0.00569 - 0.00478 * Math.Sin(Radians(x));
	}

	double MeanEclipticObliquity()
	{
		double century = JulianCentury();
		return 23.0 + (26.0 + ((21.448 - century * (46.815 + century * (0.00059 - century * 0.001813)))) / 60.0) / 60.0;
	}

	double ObliquityCorrection()
	{
		double x = 125.04 - 1934.136 * JulianCentury();
		return MeanEclipticObliquity() + 0.00256 * Math.Cos(Radians(x));
	}

	double SunDeclination()
	{
		double radians = Math.Asin(Math.Sin(Radians(ObliquityCorrection())) * Math.Sin(Radians(SunApparentLongitude())));
		return Degrees(radians);
	}

	double VarY()
	{
		double correction = ObliquityCorrection();
		return Math.Tan(Radians(correction / 2.0)) * Math.Tan(Radians(correction / 2.0));
	}

	double EquationOfTime()
	{
		double longitude = Radians(SunGeometricMeanLongitude());
		double anomaly = Radians(SunMeanAnomaly());
		double eccentricity = EccentricityOfEarthOrbit();
		double y = VarY();
		double a = 4.0 * eccentricity * y * Math.Sin(anomaly) * Math.Cos(2.0 * longitude);
		double b = 0.5 * y * y * Math.Sin(4.0 * longitude);
		double c = 1.25 * eccentricity * eccentricity * Math.Sin(Radians(SunMeanAnomaly() * 2.0));
		double x = y * Math.Sin(2.0 * longitude) - 2.0 * eccentricity * Math.Sin(anomaly + a - b - c);
		return 4.0 * Degrees(x);
	}

	double TrueSolarTime()
	{
		double timeZoneOffset = (double) _DateTimeOffset.Offset.TotalHours;
		return Mod((double)TimePastLocalMidnight() * 1440.0 + EquationOfTime() + 4.0 * Longitude - 60.0 * timeZoneOffset, 1440.0);
	}

	double HourAngle()
	{
		double angle = TrueSolarTime() / 4.0;
		return angle < 0.0 ? angle + 180.0 : angle - 180.0;
	}

	double SolarZenith()
	{
		double latitude = Radians(Latitude);
		double declination = Radians(SunDeclination());
		double hourAngle = Radians(HourAngle());
		double x = Math.Sin(latitude) * Math.Sin(declination) + Math.Cos(latitude) * Math.Cos(declination) * Math.Cos(hourAngle);
		return Degrees(Math.Acos(x));
	}

	double SolarElevation()
	{
		return 90.0 - SolarZenith();
	}

	double SolarAzimuth()
	{
		double latitude = Radians(Latitude);
		double declination = Radians(SunDeclination());
		double zenith = Radians(SolarZenith());
		double hourAngle = HourAngle();
		double x = ((Math.Sin(latitude) * Math.Cos(zenith)) - Math.Sin(declination)) / (Math.Cos(latitude) * Math.Sin(zenith));	
		double angle = Degrees(Math.Acos(x));
		return (hourAngle > 0.0) ? Reduce(angle + 180.0) : Reduce(540.0 - angle);
	}
}