#include "RunCommands.h"
#include "Robot.h"
#include "DriveByMillis.h"

#include <thread>
#include <chrono>

RunCommands::RunCommands(Robot& robot, RCM& rcm):robot(robot), rcm(rcm)
{
}
void RunCommands::UpdateAndAddComponent(Command* command)
{
	rcm.AddCommand(command);
	while (rcm.Update()) {
		robot.Update();
		//std::cout << std::string(100, '\n');
		std::cout << "Left" << robot.tempMotorEncoders[Motors::LeftDrive] << "\n";
		std::cout << "Right" << robot.tempMotorEncoders[Motors::RightDrive] << "\n";
		std::cout << "Attachment1" << robot.tempMotorEncoders[Motors::Attachment1] << "\n";
		std::cout << "Attachment2" << robot.tempMotorEncoders[Motors::Attachment2] << "\n";

		std::cout << "\n\n\n";

		std::this_thread::sleep_for(std::chrono::milliseconds(16));

	}
	robot.Update();
}

void RunCommands::RunAllTheCommands() {
	float floatVariable2 = 0;

	UpdateAndAddComponent(new DriveByMillis(75, 75, 500, MoveByMillisMode::AverageMode, vector<MyVector2>{}));
	if (floatVariable2 == 1)
	{
		UpdateAndAddComponent(new DriveByMillis(75, 0, 100, MoveByMillisMode::AverageMode, vector<MyVector2>{}));
	}
	else
	{
		UpdateAndAddComponent(new DriveByMillis(0, 75, 100, MoveByMillisMode::AverageMode, vector<MyVector2>{}));
	}
	UpdateAndAddComponent(new DriveByMillis(75, 75, 100, MoveByMillisMode::AverageMode, vector<MyVector2>{}));
	std::cin.get();
}