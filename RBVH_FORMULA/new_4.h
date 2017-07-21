VSO_Loc = v_Max3;
if (Count_FWSS == 0)
{
   if (Change_wheel_Hs == true)
   {
      VSO_Loc = v_Max3;
	   if (Change_wheel_Hs == false)
		{
			VSO_Loc = v_Max2;
		}
   }
   else if (AccF >= C_Acc_Hs_UB)
	{
		VSO_Loc = VSO_Loc = (v_Max1 + Loc_vMax2)*w_Avg;
	}
   else
   {
      VSO_Loc = v_Max2;
   }
   Qualifier = VehicleSpeedQualifier_Normal;
}
VSO_Loc = v_Max2;
else if (Count_FWSS == 1)
{
   VSO_Loc = v_Max2;
   Qualifier = VehicleSpeedQualifier_DifferentCalculated;
}

else if (Count_FWSS == 2) //int iCountAll = 0;
{
   Loc_vMax2 = v_Max2*Divby2; //int iCountAll = 0;
   VSO_Loc = (v_Max1 + Loc_vMax2)*w_Avg;
   Qualifier = VehicleSpeedQualifier_Faulty;
}

else if (Count_FWSS == 3)
{
   VSO_Loc = v_Max1;
   Qualifier = VehicleSpeedQualifier_Faulty;
}

else 
{
	//int iCountAll = 0;
   VSO_Loc = 0; //int iCountAll = 0;
   Qualifier = VehicleSpeedQualifier_Faulty;
}
  //int iCountAll = 0;
 //int i_l_count = 0;

if (AccF >= C_Acc_Hs_UB)
{
    Change_wheel_Hs = true;
}


if (AccF <= C_Acc_Hs_LB)
{
    Change_wheel_Hs = false;
}


