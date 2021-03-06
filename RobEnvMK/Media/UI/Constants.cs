//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace RobEnvMK
{
	using System;
	
	
	/// <summary>
	/// Class to keep constant values common to all classes.
	/// </summary>
	/// <remarks>
	/// 	created by - Marek Karcz
	/// 	created on - 2/28/2005 11:47:30 PM
	/// </remarks>
	public class Constants : object
	{
		//public const int SIGHT_DISTANCE = 2; //3; //6;
		public const int RADAR_RESN = 360;
		public const int RADAR_RESN2 = 180;
		public const double RADAR_RES = 360.0f;
		public const double RADAR_RES2 = 180.0f;
		public const double ANGLE_STEP = (2f * Math.PI) / RADAR_RES;
		public const double DEG2RAD = ANGLE_STEP;
        //public const double ANGLE_STEP2 = 2.0f * ANGLE_STEP;
        public const double RAD2DEG = RADAR_RES2 / Math.PI;
		public const double PI2 = 2.0f * Math.PI;
		//public const int BODY_EDGE_SCAN_STEPS = 32;
        public const int BODY_EDGE_SCAN_STEPS = 120;
		//public const double RADAR_SCAN_STEP = 0.05f;
		public const double RADAR_SCAN_STEP = 0.005f;
        //public const double RADAR_SCAN_STEP = 0.25f;
        public const int SCR_RES_X = 1024;
        public const int SCR_RES_Y = 768;
        public const float EPSILON = 0.0000000000001f;
        public const int ROBOT_MAX_IDLE_STEPS = 2000; // robot is allowed to skip (not send any command) 
        											  // this many frames in a row before it is killed as inactive
		public const int ROBOT_MAX_NO_MOVEMENT = 10000; // robot is allowed to not move out of certain radius
														// this many frames in a row before it is killed as stuck
		public const int ROBOT_STUCK_RADIUS = 300;	// this number times robot step length is the radius - if robot doesn't leave
													// this radius within number of steps defined by ROBOT_MAX_NO_MOVEMENT,
													// it is considered stuck

		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public Constants()
		{
		}

	}
}
