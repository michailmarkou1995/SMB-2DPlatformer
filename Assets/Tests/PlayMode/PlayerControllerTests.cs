using System.Collections;
using Core.Managers;
using Core.Player;
using Interfaces.Core.Managers;
using Interfaces.Core.Player;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class PlayerControllerTests
    {
        private PlayerController _player;

        [Test]
        public void Test_1_Equal_1()
        {
            Assert.AreEqual(1, 1);
        }

        [SetUp]
        public void Setup()
        {
            _player = new GameObject().AddComponent<PlayerController>();
        }
        
        [UnityTest]
        public IEnumerator Test_PC_FaceDirection_1()
        {
            // Arrange/Assign
            //PlayerController player = new GameObject().AddComponent<PlayerController>();
            // Rigidbody2D rigidbody = player.GetComponent<Rigidbody2D>();
            // rigidbody.velocity = new Vector2(0, 0);
            // player.transform.position = new Vector3(0, 0, 0);
            // BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
            // ILevelManager levelManager = Substitute.For<ILevelManager>();
            // _player.GetLevelManager.Returns(LevelManager.Instance); //LevelManager.Instance
            ILevelManager levelManager = Substitute.For<ILevelManager>();
            //_player._levelManager = levelManager;
            _player.GetLevelManager.Returns(levelManager);

            // Logs
            UnityEngine.TestTools.LogAssert.Expect(
                LogType.Log,
                "PlayerController.cs: PlayerController.GetLevelManager() returned null");

            // Act
            //player.GetMovement.FaceDirectionX = 1;

            yield return null;

            // Assert
            //Assert.AreEqual(1, player.GetMovement.FaceDirectionX);
            Assert.IsTrue(_player.GetLevelManager != null);
        }
    }
}