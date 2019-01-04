using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WROSimulatorV2
{
    public static class FieldAndRobotInfo
    {
        public static PointF RobotSize { get; private set; }//in millimeters
        public static PointF FieldSize { get; private set; }//in millimeters
        public static PointF StartRobotPos { get; private set; }//in millimeters
        public static PointF FieldImageSize { get; private set; }
        public static PointF RobotOrigin { get; private set; }//in millimeters
        public static float DistaneBetweenWheels { get; private set; }
        public static readonly float RotationOffset = 90;
        public static readonly float WheelDiamater = 64.4f;
        public static void Init(PointF fieldImageSize)
        {
            RobotSize = new PointF(250, 250);
            FieldSize = new PointF(2362, 1143);
            FieldImageSize = fieldImageSize;
            RobotOrigin = new PointF(RobotSize.X / 2, RobotSize.Y / 2);
            StartRobotPos = new PointF(FieldSize.X - (RobotSize.X - RobotOrigin.X), FieldSize.Y / 2);
            DistaneBetweenWheels = RobotSize.X;
        }
        public static PointF ToPixels(PointF millis)
        {
            return new PointF(FieldImageSize.X * millis.X / FieldSize.X, FieldImageSize.Y * millis.Y / FieldSize.Y);
        }
        public static PointF ToMillis(PointF pixels)
        {
            return new PointF(FieldSize.X * pixels.X / FieldImageSize.X, FieldSize.Y * pixels.Y / FieldImageSize.Y);
        }
        public static float DegreesToMillis(float degrees)
        {
            return (float)(Math.PI * WheelDiamater * (degrees / 360));
        }
        public static float MillisToDegrees(float millis)
        {
            return (float)((millis * 360f)/(Math.PI * WheelDiamater));
        }
    }

    public class Robot
    {
        public PointF Location { get; set; }
        public PointF Size { get; }
        public PointF Origin { get; }
        public float Rotation { get; set; }
        public Bitmap Image { get; private set; }
        public float DegreesPerSecond = 924;
        public Dictionary<Motors, Component> Components { get; private set; }
        public Dictionary<Motors, float> MotorEncoders { get; private set; }
        public Robot(Bitmap image)
        {
            Image = image;
            Location = FieldAndRobotInfo.StartRobotPos;
            Rotation = 0;
            Size = FieldAndRobotInfo.RobotSize;
            Origin = FieldAndRobotInfo.RobotOrigin;
            Components = new Dictionary<Motors, Component>();
            Components.Add(Motors.LeftDrive, new Component(Motors.LeftDrive, new MotorInfo(DegreesPerSecond)));
            Components.Add(Motors.RightDrive, new Component(Motors.RightDrive, new MotorInfo(DegreesPerSecond)));
            Components.Add(Motors.Attachment1, new Component(Motors.Attachment1, new MotorInfo(DegreesPerSecond)));
            Components.Add(Motors.Attachment2, new Component(Motors.Attachment2, new MotorInfo(DegreesPerSecond)));
            Components.Add(Motors.Other, new OtherComponent());
            MotorEncoders = new Dictionary<Motors, float>();
            foreach (var c in Components)
            {
                MotorEncoders.Add(c.Key, 0);
            }
        }
        public void ResetRobot()
        {
            Location = FieldAndRobotInfo.StartRobotPos;
            Rotation = 0;
            foreach (var c in Components.Keys)
            {
                Components[c].Power = 0;
                MotorEncoders[c] = 0;
            }
        }

        public void Update(long elapsedMillis)
        {
            UpdateEncoders(elapsedMillis);
            int leftPower = Components[Motors.LeftDrive].Power;
            int rightPower = Components[Motors.RightDrive].Power;
            if (leftPower != 0 || rightPower != 0)
            {
                float leftDistance = Components[Motors.LeftDrive].CurrentUpdateDistance;
                float rightDistance = Components[Motors.RightDrive].CurrentUpdateDistance;
                if (leftPower == rightPower)
                {
                    float distance = leftDistance;

                    Location = Location.Add(RotatePoint(new PointF(-distance, 0), Rotation));//new PointF(x, y*-1));
                }
                else
                {
                    float big = leftDistance;
                    float small = rightDistance;
                    float rotationMutiplier = -1;
                    if (rightDistance > leftDistance)
                    {
                        big = rightDistance;
                        small = leftDistance;
                        rotationMutiplier *= -1;
                    }
                    float distanceBetweenWheels = FieldAndRobotInfo.DistaneBetweenWheels;
                    float smallRadius = (small * distanceBetweenWheels) / (big - small);
                    float bigRadius = (big * distanceBetweenWheels) / (big - small);
                    float originRadius = ((distanceBetweenWheels / 2) * (big + small)) / (big - small);
                    float smallRotation = (small * 360) / (2 * (float)Math.PI * smallRadius);
                    float bigRotation = (big * 360) / (2 * (float)Math.PI * bigRadius);

                    float turnChange;
                    if (!float.IsNaN(smallRotation)) { turnChange = smallRotation; }
                    else { turnChange = bigRotation; }

                    if (!float.IsNaN(turnChange))
                    {
                        Rotation += turnChange * rotationMutiplier;
                    }

                    if (!float.IsNaN(originRadius) && originRadius != 0)
                    {
                        float originMove = (big + small) / 2;
                        float originRotation = (originMove * 360) / (2 * (float)Math.PI * originRadius);
                        float sin = (float)(Math.Sin(Extensions.ToRadians(originRotation)) * originRadius);
                        float cos = (float)(Math.Cos(Extensions.ToRadians(originRotation)) * originRadius - originRadius);
                        PointF positionChange = new PointF(-sin, cos);
                        Location = Location.Add(RotatePoint(positionChange, Rotation));
                    }
                }
            }
        }

        public void UpdateEncoders(long elapsedMillis)
        {
            foreach (var m in Components.Keys)
            {
                MotorEncoders[m] += Components[m].GetUpdateDistance(elapsedMillis);
            }
        }

        PointF RotatePoint(PointF point, float rotation)
        {
            double distance = point.Distance(new PointF(0, 0));
            double currentRad = Math.Atan2(point.Y, point.X);
            double rad = -Extensions.ToRadians(rotation) + currentRad;
            float x = (float)(Math.Cos(rad) * distance);
            float y = (float)(Math.Sin(rad) * distance);
            return new PointF(x, y);
        }

        float GetOriginRadius(float largeDistance, float smallDistance, float distanceBetween)
        {
            float distance = largeDistance;
            float halfDistanceBetween = distanceBetween / 2;
            float originDistance = (largeDistance + smallDistance) / 2;
            float multiplier = 1;
            if (distance == 0)
            {
                distance = smallDistance;
                halfDistanceBetween *= -1;
                multiplier *= -1;
            }
            float originOverDistance = (originDistance / distance);
            return multiplier * (originOverDistance * halfDistanceBetween) / (1 - originOverDistance);
        }

        float GetInnerRadius(float largeDistance, float smallDistance, float distanceBetween)
        {
            return (distanceBetween * smallDistance) / (largeDistance - smallDistance);
        }

        public void Draw(Graphics gfx)
        {
            RectangleF rectangle = new RectangleF(FieldAndRobotInfo.ToPixels(Location.Subtract(Origin)), FieldAndRobotInfo.ToPixels(Size).ToSize());
            var pts = Extensions.GetDestinationPoints(rectangle, FieldAndRobotInfo.ToPixels(Origin), Rotation + FieldAndRobotInfo.RotationOffset);
            gfx.DrawImage(Image, pts);
        }
    }
}
