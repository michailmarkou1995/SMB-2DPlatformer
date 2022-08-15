using System.Collections;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class DestroyOutOfScreenTests
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Test_DOS_CreateObject()
        {
            // Arrange
            var gameObject = new GameObject();
            yield return null;
            // Act
            Assert.IsNotNull(gameObject);
        }

        [UnityTest]
        public IEnumerator Test_DOS_DestroyObject()
        {
            // Arrange
            var gameObject = new GameObject();
            var destroyOutOfScreen = new GameObject().AddComponent<DestroyOutOfScreen>();

            // Act
            destroyOutOfScreen.DestroyGameObj(gameObject);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            // Assert
            Assert.IsTrue(gameObject == null);
        }
    }
}