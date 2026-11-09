using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace OmiLAXR.Pseudonymization.Tests
{
    public class RandomPseudonymGenerator_Tests
    {
        private const string PseudonymKey = "pseudonym";
        private string _storedPseudonym;
        private readonly List<GameObject> _created = new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            // The behaviour persists pseudonyms in PlayerPrefs, keep the developer's value intact.
            _storedPseudonym = PlayerPrefs.HasKey(PseudonymKey) ? PlayerPrefs.GetString(PseudonymKey) : null;
            PlayerPrefs.DeleteKey(PseudonymKey);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var go in _created)
                Object.DestroyImmediate(go);
            _created.Clear();

            if (_storedPseudonym == null)
                PlayerPrefs.DeleteKey(PseudonymKey);
            else
                PlayerPrefs.SetString(PseudonymKey, _storedPseudonym);
            PlayerPrefs.Save();
        }

        private RandomPseudonymGenerator CreateGenerator(bool generateOnStartup, bool remember = false)
        {
            var go = new GameObject("Pseudonym Generator");
            go.SetActive(false);
            _created.Add(go);
            var generator = go.AddComponent<RandomPseudonymGenerator>();
            generator.generateOnStartup = generateOnStartup;
            generator.rememberPseudonym = remember;
            return generator;
        }

        [Test]
        public void RandomPseudonymGenerator_Generates_Six_Uppercase_Alphanumerics()
        {
            // Arrange
            var generator = CreateGenerator(generateOnStartup: false);
            var pseudonyms = new List<string>();
            generator.OnCreatedPseudonym += pseudonyms.Add;

            // Act
            generator.Generate();
            generator.Generate();

            // Assert
            Assert.AreEqual(2, pseudonyms.Count);
            foreach (var p in pseudonyms)
                StringAssert.IsMatch("^[A-Z0-9]{6}$", p);
        }

        [Test]
        public void RandomPseudonymGenerator_Generates_On_Startup_And_Becomes_Instance()
        {
            // Arrange
            var generator = CreateGenerator(generateOnStartup: true);
            string pseudonym = null;
            generator.OnCreatedPseudonym += p => pseudonym = p;

            // Act
            generator.gameObject.SetActive(true);

            // Assert
            Assert.AreEqual(generator, RandomPseudonymGenerator.Instance);
            Assert.IsNotNull(pseudonym);
        }

        [UnityTest]
        public IEnumerator RandomPseudonymGenerator_Second_Instance_Is_Destroyed()
        {
            // Arrange
            var first = CreateGenerator(generateOnStartup: false);
            first.gameObject.SetActive(true);
            var second = CreateGenerator(generateOnStartup: false);

            // Act
            second.gameObject.SetActive(true);
            yield return null;

            // Assert
            Assert.AreEqual(first, RandomPseudonymGenerator.Instance);
            Assert.IsTrue(second == null);
        }

        [Test]
        public void PseudonymBehaviour_Remembers_Pseudonym()
        {
            // Arrange
            var generator = CreateGenerator(generateOnStartup: false, remember: true);
            var pseudonyms = new List<string>();
            generator.OnCreatedPseudonym += pseudonyms.Add;

            // Act
            generator.Generate();
            generator.Generate();

            // Assert
            Assert.AreEqual(2, pseudonyms.Count);
            Assert.AreEqual(pseudonyms[0], pseudonyms[1]);
            Assert.AreEqual(pseudonyms[0], generator.LoadPseudonym());
        }

        [Test]
        public void PseudonymBehaviour_Load_And_Save()
        {
            // Arrange
            var generator = CreateGenerator(generateOnStartup: false);

            // Assert
            Assert.IsNull(generator.LoadPseudonym());

            // Act
            generator.SavePseudonym("ABC123");

            // Assert
            Assert.AreEqual("ABC123", generator.LoadPseudonym());
        }

        [Test]
        public void PseudonymBehaviourPrinter_Logs_Created_Pseudonym()
        {
            // Arrange
            var generator = CreateGenerator(generateOnStartup: false);
            var printerObject = new GameObject("Printer");
            printerObject.SetActive(false);
            _created.Add(printerObject);
            printerObject.AddComponent<PseudonymBehaviourPrinter>().pseudonymBehaviour = generator;
            printerObject.SetActive(true);
            LogAssert.Expect(LogType.Log, new Regex("Generated pseudonym from .*RandomPseudonymGenerator.*"));

            // Act
            generator.Generate();
        }
    }
}
