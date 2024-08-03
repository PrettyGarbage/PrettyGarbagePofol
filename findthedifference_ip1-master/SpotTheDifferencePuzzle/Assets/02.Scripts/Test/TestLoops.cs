//using System;
//using Firebase.TestLab;
//using UnityEngine;

//public class TestLoops : MonoBehaviour {
// private TestLabManager _testLabManager;

// private const int BootTimeTest = 1;

// private const int FrameRateAfterOneSecondTest = 2;
// private const int FrameRateAfterThreeSecondsTest = 3;

// void Start() {
//   _testLabManager = TestLabManager.Instantiate();
// }

// // Update is called once per frame
// void Update() {
//   // only change the behavior of the app if there is a scenario being tested
//   if (!_testLabManager.IsTestingScenario) return;

//   switch (_testLabManager.ScenarioNumber) {
//     case BootTimeTest:
//       // print out the result and complete       
//       _testLabManager.LogToResults("Time to boot: " +                        
//                                    Time.realtimeSinceStartup + 
//                                    "\n");
//       _testLabManager.NotifyHarnessTestIsComplete();       
//       break;
//     case FrameRateAfterOneSecondTest:
//       CheckFramerate(1.0f);
//       break;
//     case FrameRateAfterThreeSecondsTest:
//       CheckFramerate(3.0f);
//       break;
//     default:
//       throw new ArgumentOutOfRangeException();
//   }
// }

// private void CheckFramerate(float afterTime) {
//   if (!(Time.time > afterTime)) return;

//   const string s = "Number frames after {0} seconds: {1}\n";
//   string output = string.Format(s, afterTime, Time.frameCount);

//   _testLabManager.LogToResults(output);
//   _testLabManager.NotifyHarnessTestIsComplete();
// }
//}