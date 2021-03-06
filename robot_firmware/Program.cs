﻿using System;
using System.Device.Gpio;
using System.Threading;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using Iot.Device.ServoMotor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace robot_firmware
{
    class Program
    {
        private static HubConnection connection;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Robot Arm Engaged!!!!");
            
            using PwmChannel pwmChannel1 = PwmChannel.Create(0,0,50);
            using ServoMotor servoMotor1 = new ServoMotor(
            pwmChannel1,
            180,
            1000,
            2000);            
           
            using PwmChannel pwmChannel2 = PwmChannel.Create(0,1,50);
            using ServoMotor servoMotor2 = new ServoMotor(
            pwmChannel2,
            180,
            1000,
            2000);

            using SoftwarePwmChannel pwmChannel3 = new SoftwarePwmChannel(27, 50, 0.5);
            using ServoMotor servoMotor3 = new ServoMotor(
            pwmChannel3,
            180,
            1000,
            2000);

            using SoftwarePwmChannel pwmChannel4 = new SoftwarePwmChannel(22, 50, 0.5);
            using ServoMotor servoMotor4 = new ServoMotor(
            pwmChannel4,
            180,
            1000,
            2000);

                      
            servoMotor1.Start();
            servoMotor2.Start();
            servoMotor3.Start();
            servoMotor4.Start();

            connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.200:4080/chathub",conf =>
                {
                    conf.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    };                    
                })
                .Build();
          

try
{
    connection.On<string, string>("ReceiveMessage", (user, message) =>
    {


        if (user == "servo1")
        {
            MoveToAngle(servoMotor1, Int32.Parse(message));
        }
        if (user == "servo2")
        {
            MoveToAngle(servoMotor2, Int32.Parse(message));
        }
        if (user == "servo3")
        {
            MoveToAngle(servoMotor3, Int32.Parse(message));
        }
        if (user == "servo4")
        {
            MoveToAngle(servoMotor4, Int32.Parse(message));
        }
        
        Console.WriteLine($"{message} posted by: {user}");
    });
    
    await connection.StartAsync();
}
catch (System.Exception)
{
    
    throw;
}

try
{
    while(true)
    {
        
    }
}
finally
{
    servoMotor1.Stop();
    servoMotor2.Stop();
    servoMotor3.Stop();
    servoMotor4.Stop();
}
        }

        static void MoveToAngle(ServoMotor Servo, int Angle) {
            Servo.WriteAngle(Angle);            
        }
    }
}
