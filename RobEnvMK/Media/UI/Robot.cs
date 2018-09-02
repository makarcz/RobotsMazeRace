/*
 * Created by SharpDevelop.
 * User: mkarcz
 * Date: 2005*02*21
 * Time: 4:45 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
//#define TRACE

using System;
using System.Diagnostics;

namespace RobEnvMK
{
	
	/// <summary>
	/// Robot interface class to the Robot Environment.
	/// </summary>
	public class Robot
	{
		double m_fInitX = 0.0f;
		double m_fInitY = 0.0f;
		int m_nPosX = 0;
		int m_nPosY = 0;
		double m_fXPos = 0f;
		double m_fYPos = 0f;
		double m_fPrevXPos = 0f;
		double m_fPrevYPos = 0f;		
		double m_fStep = 0.1f;
		double m_fSize = 1.0f;
		double m_fAngle = 0.0f;
		MainClass m_World = null;
		bool m_bKilled = false;
        bool m_bReset = false;
		double [] m_faSinTbl = new double [Constants.RADAR_RESN];
		double [] m_faCosTbl = new double [Constants.RADAR_RESN];
        string m_sLastCommand = string.Empty;
        int m_nNum;
        string m_sName = string.Empty;
        
        public double PosX {
        	
        	get { return m_fXPos; }
        }
        
        public double PosY {
        	
        	get { return m_fYPos; }
        }
        
        public double PrevXPos {
        	
        	get { return m_fPrevXPos; }
        }
        
        public double PrevYPos {
        	
        	get { return m_fPrevYPos; }
        }

        public bool isKilled
        {
        	get { return (m_bKilled); }
        }
        
        public string LastCommand
        {
            get { return (m_sLastCommand); }
        }

        public bool ResetRobot
        {
            get { return (m_bReset); }
            set { m_bReset = value; }
        }
        
        public string Name
        {
        	get { return m_sName; }
        }
		
		public Robot ()
		{
			m_fStep = 0.6f;
			m_fXPos = 0.0f;
			m_fYPos = 0.0f;
			m_fAngle = 0.0f;
			m_nPosX = (int) m_fXPos; // / m_fSize);
			m_nPosY = (int) m_fYPos; // / m_fSize);
			m_World = null;
			Initialize ();			
		}
		
		public Robot (float x, double y, double step, double size, MainClass world, int num, string name)
		{
			m_fStep = step;
			m_fSize = size;
			m_fXPos = x;
			m_fYPos = y;
			m_fInitX = x;
			m_fInitY = y;
			m_fAngle = 0.0f;
			m_nPosX = (int) x; // / size);
			m_nPosY = (int) y; // / size);
			m_World = world;
			m_nNum = num;
			m_sName = name;
			Initialize ();
		}		
		
		void Initialize ()
		{
			double angle = 0.0f;
			for (int i = 0; i < Constants.RADAR_RESN; i++, angle += Constants.ANGLE_STEP)
			{
				if (null == m_World)
				{
					m_faSinTbl [i] = Math.Sin (angle);
					m_faCosTbl [i] = Math.Cos (angle);
				}
				else
				{
					m_faSinTbl [i] = m_World.GetSinFromTbl (i);
					m_faCosTbl [i] = m_World.GetCosFromTbl (i);
				}
			}
			Trace.WriteLine ("Robot initialized.");
		}
		
		/*
		 * World / environment communicates with the robots via stdio.
		 * Bots are sending commands to the environment and environment takes
		 * actions and sends responses when it is expected.
		 * All the responses are returned as strings as well.
		 * 
		 * Following bot control / query commands are currently supported:
		 * 
		 * Move - moves the bot one step ahead
		 * 
		 * Turn [angle] - rotates the robot by angle [rad] to the right (angle < 0)
		 * 				  or to the left (angle >= 0). Note that this is a relative
		 * 				  angle of turn to the current robot's absolute angle in the
		 * 				  environment's coordinates system.
		 * 
		 * Kill - bot commits suicide
		 * 
		 * ?Radar - query the radar function, will return a list of 360 floating point
		 *          values, where value at step #0 is the distance to the nearest
		 *          obstacle exactly behind the robot, value at step #1 is the distance
		 * 			to the obstacle at 1 degree, value at step #2 is the distance to
		 * 			the obstacle at 2 degrees and so on, going counter-clockwise.
		 * 			Value at step #180 represents distance to the obstacle in front of
		 * 			the bot. Value at step #90, to the right, at step #270 to the left.
		 * 			You should get the idea by now.
		 * 
		 * ?PRadar <begin> <steps> - query the radar function but instead of full circle
		 * 							 (360 deg), query from <begin> angle for a number
		 * 							 of <steps>.
		 *           
		 * ?Size - returns the size of the robot (diameter = 2 * r) in the world's
		 * 		   absolute units. The size of the world is determined by the resolution
		 * 		   of world's map bitmap file, from which all the radar rays are
		 * 		   calculated.
		 * 
		 * ?Killed - check if the robot was killed in previous step, returns 'true'
		 * 			 or 'false' string.
		 * 
		 * ?Eval dist - query the points located at <dist> distance from the robot
		 * 				for their distances from the labirynth exit.
		 * 			    It's like a ?Radar, but instead of returning distances to the
		 * 				obstacles, returns the negative values of distances of these
		 * 				points to the exit. Robot program code can use these values
		 * 				to decide its next step while looking for the exit in its
		 * 				algorithm (the greater the value returned from ?Eval, the closer
		 * 				given point is to the exit).
		 * 
		 * ?Eval angle dist - similar to above, however queries only a single point
		 * 					  at angle <angle> radians and <dist> distance from the
		 * 					  bot for that point's distance from exit, returns single
		 * 					  value.
		 * 
		 * Reset - causes robot coordinates to reset to their initial location and
		 * 		   rotate to 0 angle position (absolute angle of the environment's
		 * 		   coordinates).
		 * 
		 * ?Step - returns the length of a step (for Move command) from setup.
		 * 		   Step can be smaller than the unit of the world's map resolution.
		 * 		   In other words, it is a floating point value and can be less than 0.
		 * 						
		 */
		public string ProcessCommand (string sCmd)
		{
            m_sLastCommand = sCmd;
			Trace.WriteLine ("Processing command: " + sCmd);
			Trace.WriteLine ("Current coordinates: [" + m_fXPos + ", " + m_fYPos + "]");
			Trace.WriteLine ("Current angle: " + m_fAngle + " rad.");
			var sRet = new MkString ();
			if (sCmd.StartsWith("Move", StringComparison.Ordinal)) {
				
				Move();
				
			} else if (sCmd.StartsWith("Kill", StringComparison.Ordinal)
			           || sCmd.StartsWith("Exit", StringComparison.Ordinal) ) {
				
				m_bKilled = true;
				
			} else if (sCmd.StartsWith("Turn", StringComparison.Ordinal)) {
				
				string[] saSplit = null;
				const string sDelim = " ";
				char[] delim = sDelim.ToCharArray();				
				saSplit = sCmd.Split(delim, 2);
				double fAngle = Convert.ToDouble(saSplit[1]);
				Turn(fAngle);
				
			} else if (sCmd.StartsWith("?Radar", StringComparison.Ordinal)) {
				
				sRet.Set(m_World.GetRadarAt(m_fXPos, m_fYPos, m_fAngle));
				
			} else if (sCmd.StartsWith("?PRadar", StringComparison.Ordinal)) {
				
				string[] saSplit = null;
				const string sDelim = " ";
				char[] delim = sDelim.ToCharArray();				
				saSplit = sCmd.Split(delim, 3);
				int begin = Convert.ToUInt16(saSplit[1]);
				int steps = Convert.ToUInt16(saSplit[2]);
				sRet.Set(m_World.GetPartialRadarAt(m_fXPos, m_fYPos, m_fAngle, begin, steps));
				
			} else if (sCmd.StartsWith("?Size", StringComparison.Ordinal)) {
				
				sRet.Set(m_World.m_Config.GetRobotSize().ToString());
				
			} else if (sCmd.StartsWith("?Killed", StringComparison.Ordinal)) {
				
				sRet.Set((m_bKilled) ? "true" : "false");
				
			} else if (sCmd.StartsWith("?Eval", StringComparison.Ordinal)) {
				
				sRet.Set(CalcEval(sCmd));
				
			} else if (sCmd.StartsWith("Reset", StringComparison.Ordinal)) {
				
				m_bKilled = false;
				m_fXPos = m_fInitX;
				m_fYPos = m_fInitY;
				m_nPosX = (int)m_fXPos;
				m_nPosY = (int)m_fYPos;
				m_bReset = true;
				m_fAngle = 0.0f;
				
			} else if (sCmd.StartsWith("?Step", StringComparison.Ordinal)) {
				
				sRet.Set(m_World.m_Config.GetStep().ToString());
			}
            if (0 < sRet.Length)
            {
                if (!sCmd.StartsWith("?Radar", StringComparison.Ordinal)
				    && !sCmd.StartsWith("?PRadar", StringComparison.Ordinal)
            	   )
                    Trace.WriteLine("Return value: " + sRet.Buffer);
                else
                    Trace.WriteLine("Return value: " + m_World.m_sRadarDetailed.Buffer);
            }
			
			return (sRet.Buffer);
		}
		
		private int NormalizeRadarIndex (int index)
		{
			int nIndex = index;
			
            while (0 > nIndex)
                nIndex += Constants.RADAR_RESN;
            while (Constants.RADAR_RESN <= nIndex)
                nIndex -= Constants.RADAR_RESN;			
            
            return (nIndex);
		}
		
		/*
		 * Calculates the distance of a point to the exit of the labirynth.
		 * 	sCmd - command
		 * There are 2 formats of ?Eval command:
		 * 
		 * 1. ?Eval <dist>
		 *    This command returns 360 strings representing the floating
		 * 	  point values of the distances of the corresponding points around
		 *    the robot to the exit.
		 * 	  The <dist> argument is the distance of the points being queried
		 * 	  from the robot. The 1-st value represents the distance from the
		 *    point at angle 0 (behind robot) from robot to the exit, 2-nd value
		 * 	  is the distance from point at angle 1 (counter-clockwise)
		 *	  and so on.
		 * 
		 * 2. ?Eval <angle> <dist>
		 *    Returns only single string representing a floating point value
		 *    of a distance of the point to the exit. The point is located
		 * 	  at distance <dist> and angle <angle> from the robot. The angle
		 * 	  is provided in radians and starts behind the robot increasing
		 * 	  in the counter-clockwise direction.
		 * 
		 */
		string CalcEval (string sCmd)
		{
			var sRet = new MkString ();
			double fEval = -1.0e6;
			double fAngle = 0.0f;
			double fDist = 0.0f;
			double fX = 0.0f;
			double fY = 0.0f;
	    	string [] saSplit = null;
			const string sDelim = " ";
	    	char [] delim = sDelim.ToCharArray ();				
	    	saSplit = sCmd.Split (delim, 3);
	    	if (2 < saSplit.Length)
	    	{ // 2 parametes eval, query single point
	        	fAngle = Convert.ToDouble (saSplit [1]);
                int na = (int) Math.Round (fAngle * Constants.RAD2DEG);
                na = NormalizeRadarIndex (na);
	        	fDist = Convert.ToDouble (saSplit [2]);
	        	fX = m_fXPos + (fDist * m_faCosTbl [na]);
	        	fY = m_fYPos + (fDist * m_faSinTbl [na]);
                fEval = -(fX * fX + fY * fY);
	        	sRet.Set (fEval.ToString ());
	    	}
			else
			{ // 1 parameter eval, query whole bunch
	        	fDist = Convert.ToDouble (saSplit [1]);
				const int na = Constants.RADAR_RESN2;
	        	for (int i = 0, j = na; Constants.RADAR_RESN > i; i++, j++)
	        	{
					if (Constants.RADAR_RESN <= j)
						j = 0;	        		
	        		fX = m_fXPos + (fDist * m_faCosTbl [j]);
	        		fY = m_fYPos + (fDist * m_faSinTbl [j]);
                    fEval = -(fX * fX + fY * fY);
	            	sRet.Append (fEval.ToString ());
	            	if (Constants.RADAR_RESN - 1 > i)
	            		sRet.AddSpace ();
	        	}
			}			
			
			return (sRet.Buffer);
		}
		
		/// <summary>
		/// Change Robot's coordinates depending on the direction the Robot
		/// is facing.
		/// </summary>
		public void Move ()
		{
			m_fPrevXPos = m_fXPos;
			m_fPrevYPos = m_fYPos;
			if (Collision (m_fXPos, m_fYPos, m_fAngle)) {
				m_World.RaceReportEvent(global::RobEnvMK.Properties.Resources.RobotNumber + m_nNum 
				                        + ", " + global::RobEnvMK.Properties.Resources.Text_Name01 + " " + m_sName
				                        + " " + global::RobEnvMK.Properties.Resources.Text_KilledAt01
				                        + " [" + m_fXPos + ", " + m_fYPos + "], "
				                        + global::RobEnvMK.Properties.Resources.Text_Angle01 + " " + m_fAngle + " [rad]");
				m_bKilled = true;
			} else {
				//if (m_fXPos < m_fSize * 3 && m_fYPos < m_fSize * 3) {
				if (Math.Sqrt(Math.Pow(m_fXPos, 2) + Math.Pow(m_fYPos, 2)) < m_fSize * 4.0) {
					m_World.StatusLog(global::RobEnvMK.Properties.Resources.Text_SuccessGoalReached);
					m_World.RaceReportSuccess(m_nNum, m_sName);
					m_bKilled = true;
				}
			}
		}
		
		/// <summary>
		/// Collision detection.
		/// </summary>
		/// <param name="xb">current robot's X coordinate</param>
		/// <param name="yb">current robot's Y coordinate</param>
		/// <param name="angle"></param>
		/// <returns>true if collision detected in the next step</returns>
		bool Collision (double xb, double yb, double angle)
		{
			bool bRet = false;
			double fX = xb + (m_fStep * Math.Cos (angle));
			double fY = yb + (m_fStep * Math.Sin (angle));
			int x = (int) fX;
			int y = (int) fY;
			
			bRet =  (0 != m_World.m_anWorldMap [x, y]);
			if (!bRet) {
				bRet = CheckBodyInCollision(fX, fY, m_World.m_Config.GetRobotSize());
			}
			if (!bRet) {
				m_fXPos = fX;
				m_fYPos = fY;
				m_nPosX = x;
				m_nPosY = y;
			} else {
				m_World.StatusLog(global::RobEnvMK.Properties.Resources.Text_CollisionDump);
				m_World.StatusLog("*  fX = " + fX);
				m_World.StatusLog("*  fY = " + fY);
				m_World.StatusLog("*   x = " + x);
				m_World.StatusLog("*   y = " + y);
				m_World.StatusLog("*   m_World.m_anWorldMap[" + x + "," + y + "] = " + m_World.m_anWorldMap[x, y]);
				
				Trace.WriteLine("COLLISION DUMP:");
				Trace.WriteLine("   fX = " + fX);
				Trace.WriteLine("   fY = " + fY);
				Trace.WriteLine("    x = " + x);
				Trace.WriteLine("    y = " + y);
				Trace.WriteLine("    m_World.m_anWorldMap[" + x + "," + y + "] = " + m_World.m_anWorldMap[x, y]);				
			}
			
			return (bRet);
		}
		
		///<summary>
		/// Check if any part of robot's body is in collision with obstacle
		/// in the next step.
		/// </summary>
		/// <param name="x">robot's x coordinate in the next step</param>
		/// <param name="y">robot's y coordinate in the next step</param>
		/// <param name="size">the size of robot</param>
		/// <returns>bool true if body is in collision with obstacle</returns>
		private bool CheckBodyInCollision (double x, double y, double size)
		{
			bool bRet = false;
			double r = size / 2.0f;
			
            for (int na = 0; Constants.RADAR_RESN > na; na++)
			{
                double fX = x + (r * m_faCosTbl[na]);
                double fY = y + (r * m_faSinTbl[na]);
				int x2 = (int) fX;
				int y2 = (int) fY;
				bRet =  (0 != m_World.m_anWorldMap [x2, y2]);
				if (bRet) {
					m_World.StatusLog(global::RobEnvMK.Properties.Resources.Text_RobotBodyInCollisionDump);
					m_World.StatusLog(global::RobEnvMK.Properties.Resources.Text_EdgeCollidedWithWallAtStep + " " + na);
					m_World.StatusLog("*  fX = " + fX);
					m_World.StatusLog("*  fY = " + fY);
					m_World.StatusLog("*  x2 = " + x2);
					m_World.StatusLog("*  y2 = " + y2);
					m_World.StatusLog("*  m_World.m_anWorldMap [" + x2 + "," + y2 + "] = " + m_World.m_anWorldMap[x2, y2]);
					
					Trace.WriteLine("ROBOT BODY IN COLLISION, DUMP:");
					Trace.WriteLine("   Edge collided with wall at step: " + na);
					Trace.WriteLine("   fX = " + fX);
					Trace.WriteLine("   fY = " + fY);
					Trace.WriteLine("   x2 = " + x2);
					Trace.WriteLine("   y2 = " + y2);
					Trace.WriteLine("   m_World.m_anWorldMap [" + x2 + "," + y2 + "] = " + m_World.m_anWorldMap[x2, y2]);
					
					break;
				}
			}
			
			return (bRet);
		}
		
		public void Turn (double fAngle)
		{
            m_fAngle += fAngle;
            while (Constants.PI2 <= m_fAngle)
                m_fAngle -= Constants.PI2;
			while (0 > m_fAngle)
				m_fAngle += Constants.PI2;
		}
		
		public double GetPosX ()
		{
			return (m_fXPos);
		}
		
		public double GetPosY ()
		{
			return (m_fYPos);
		}
		
		public double GetAngle ()
		{
			return (m_fAngle);
		}
		
		public void SetPosX (double x)
		{
			m_fXPos = x;
		}
		
		public void SetPosY (double y)
		{
			m_fYPos = y;
		}	
		
		public double GetCurrDir ()
		{
			return (m_fAngle);
		}
	}
}
