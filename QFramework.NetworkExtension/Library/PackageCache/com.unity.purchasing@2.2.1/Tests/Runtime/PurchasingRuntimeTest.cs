#if UNITY_PURCHASING || UNITY_UNIFIED_IAP
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Purchasing;
using UnityEditor.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Test
{
	public class PurchasingRuntimeTest
    {
        private IList<ProductDefinition> products;
		private int transactionID;
		private TransactionLog transactionLog;
		private List<ProductDescription> GetProductDescriptions(IEnumerable<ProductDefinition> products) {
			return (from product in products
					let receipt = "fakeReceipt"
					let tranID = (transactionID++).ToString()
					let metadata = new ProductMetadata ("Fake", "Fake", "Fake", "GBP", 1.23m)
				select new ProductDescription (product.storeSpecificId, metadata, receipt, tranID)).ToList();
		}

		private ProductMetadata metadata;
		private bool _isPurchasingEnabled;

		[OneTimeSetUpAttribute]
    	public void BeforeAll()
    	{
        	_isPurchasingEnabled = PurchasingSettings.enabled;
        	if (!_isPurchasingEnabled)
        	{
            	Debug.Log("Temporarily Enabling Purchasing for tests");
        	}
       	 	PurchasingSettings.enabled = true;
    	}

    	[OneTimeSetUpAttribute]
    	public void AfterAll()
    	{
        	if (!_isPurchasingEnabled)
        	{
            	Debug.Log("Purchasing tests complete.  Purchasing will now be disabled");
        	}
        	PurchasingSettings.enabled = _isPurchasingEnabled;
    	}

        [SetUp]
        public void Init()
        {
			metadata = new ProductMetadata("£1.23", "Fake title", "Fake desc", "GBP", 1.23m);

            products = new List<ProductDefinition> ();
            products.Add (new ProductDefinition ("ammo",  "ammo.ios", ProductType.Consumable));
            products.Add (new ProductDefinition ("bomb",  "bomb.ios", ProductType.Consumable));
            products.Add (new ProductDefinition ("sword", "sword.ios", ProductType.NonConsumable));
            products.Add (new ProductDefinition ("nanogenes", "nanogenes.ios", ProductType.Subscription));

        }

		[Test]
		public void TestProductMetaData()
		{
			Assert.AreEqual(metadata.localizedPriceString, "£1.23");
            Assert.AreEqual(metadata.localizedTitle, "Fake title");
            Assert.AreEqual(metadata.localizedDescription, "Fake desc");
            Assert.AreEqual(metadata.isoCurrencyCode, "GBP");
            Assert.AreEqual(metadata.localizedPrice, 1.23m);
		}

		[Test]
		public void TestProductDescription()
		{
			var prod1 = new ProductDescription("testID", metadata);
            Assert.AreEqual(prod1.storeSpecificId, "testID");
            Assert.AreEqual(prod1.type, ProductType.Consumable);
            Assert.AreEqual(prod1.metadata, metadata);
            Assert.That(prod1.receipt, Is.Null);
            Assert.That(prod1.transactionId, Is.Null);
		}

        [Test]
        public void TestProductConsumables()
        {
            Assert.IsNotEmpty(products);
            Assert.AreEqual(products.Count, 4);
            var product = products[0];
            Assert.AreEqual(product.id, "ammo");
            Assert.AreEqual(product.storeSpecificId, "ammo.ios");
            Assert.AreEqual(product.type, ProductType.Consumable);
        }

        private static string GetTempFolder() {
            var path = Path.Combine (Directory.GetCurrentDirectory (), "Test/tmp");
            if (Directory.Exists (path)) {
                Directory.Delete (path, true);
            }
            Directory.CreateDirectory(path);
            return path;
        }

        class DummyProducts
        {
		    public static ProductDefinition Consumable =
			    new ProductDefinition ("coins", "com.brainDeadDesign.DepletedUraniumBullets", ProductType.Consumable, true, new List<PayoutDefinition> { new PayoutDefinition (PayoutType.Currency, "gold", 123), new PayoutDefinition (PayoutType.Resource, "health", 100) });

		    public static ProductDefinition NonConsumable =
			    new ProductDefinition("Rotary Cannon", "com.brainDeadDesign.GAU-12Equalizer", ProductType.NonConsumable, true, new PayoutDefinition(PayoutType.Item, "Cannon", 1, "anti-materiel cannon"));

		    public static ProductDefinition Subscription =
			    new ProductDefinition("subscription", "com.brainDeadDesign.subscription", ProductType.Subscription);

		    public static HashSet<ProductDefinition> Products = new HashSet<ProductDefinition>
		    {
			    Consumable,
			    NonConsumable,
			    Subscription
		    };

		    public static ReadOnlyCollection<ProductDefinition> ProductCollection =
			    new ReadOnlyCollection<ProductDefinition> (Products.ToList());

		    public static ProductMetadata DummyMetadata =
			    new ProductMetadata("£1.99", "Dummy product title", "Dummy product description", "GBP", 1.99m);
	    }
    }
}
#endif
