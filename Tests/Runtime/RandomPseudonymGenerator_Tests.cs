using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using OmiLAXR.Pseudonymization.Helper;
using OmiLAXR.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace OmiLAXR.Pseudonymization.Tests
{
    public sealed class InMemoryStore : IKeyValueStore
    {
        public readonly Dictionary<string, string> Values = new Dictionary<string, string>();
        public int Saves;

        public bool HasKey(string key) => Values.ContainsKey(key);
        public string GetString(string key) => Values[key];
        public void SetString(string key, string value) => Values[key] = value;
        public void DeleteKey(string key) => Values.Remove(key);
        public void Save() => Saves++;
    }

    public sealed class FakeHttpClient : IHttpClient
    {
        public readonly List<string> Requests = new List<string>();
        public string Response = "{\"pseudonym\":\"BraveOtter\"}";

        public Task<string> GetStringAsync(string uri)
        {
            Requests.Add(uri);
            return Task.FromResult(Response);
        }
    }

    public class RandomPseudonymGenerator_Tests
    {
        private readonly List<GameObject> _created = new List<GameObject>();
        private InMemoryStore _store;

        [SetUp]
        public void SetUp()
        {
            // An in-memory store keeps the developer's PlayerPrefs untouched
            _store = new InMemoryStore();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var go in _created)
                Object.DestroyImmediate(go);
            _created.Clear();
        }

        private T CreateBehaviour<T>(bool generateOnStartup, bool remember = false) where T : PseudonymBehaviour
        {
            var go = new GameObject(typeof(T).Name);
            go.SetActive(false);
            _created.Add(go);
            var behaviour = go.AddComponent<T>();
            behaviour.generateOnStartup = generateOnStartup;
            behaviour.rememberPseudonym = remember;
            behaviour.Store = _store;
            return behaviour;
        }

        private RandomPseudonymGenerator CreateGenerator(bool generateOnStartup, bool remember = false)
            => CreateBehaviour<RandomPseudonymGenerator>(generateOnStartup, remember);

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
            Assert.AreEqual(pseudonyms[0], _store.Values["pseudonym"]);
            Assert.AreEqual(1, _store.Saves);
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

            // Act
            generator.Store = null;

            // Assert
            Assert.AreSame(PlayerPrefsStore.Instance, generator.Store);
        }

        [Test]
        public void PseudonymProvider_Fetches_Pseudonym_Once_Per_Generate()
        {
            // Arrange
            var provider = CreateBehaviour<PseudonymProvider>(generateOnStartup: false);
            var http = new FakeHttpClient();
            provider.HttpClient = http;
            provider.baseUrl = "https://example.org/pseudo/";
            provider.languageCode = "de";
            var pseudonyms = new List<string>();
            provider.OnCreatedPseudonym += pseudonyms.Add;

            // Act
            provider.Generate();

            // Assert
            CollectionAssert.AreEqual(new[] { "https://example.org/pseudo/de?capitalize=True" }, http.Requests);
            CollectionAssert.AreEqual(new[] { "BraveOtter" }, pseudonyms);
        }

        [Test]
        public void PseudonymProvider_Failed_Request_Yields_Null()
        {
            // Arrange
            var provider = CreateBehaviour<PseudonymProvider>(generateOnStartup: false);
            provider.HttpClient = new FakeHttpClient { Response = null };
            var pseudonyms = new List<string>();
            provider.OnCreatedPseudonym += pseudonyms.Add;

            // Act
            provider.Generate();

            // Assert
            CollectionAssert.AreEqual(new string[] { null }, pseudonyms);
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
