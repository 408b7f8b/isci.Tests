using System;
using System.ComponentModel;
using System.Linq;
using isci.Daten;
using Xunit;
using Xunit.Abstractions;

namespace isci.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test_Dateneintrag()
        {
            var dtb = new isci.Daten.dtBool(false, "bool");
            Assert.True(dtb.Identifikation == "bool");

            Assert.False(dtb.Wert);
            var b = dtb.WertNachBytes();
            Assert.True(b.Count() == 1);
            Assert.True(b[0] == 0);

            dtb.Wert = true;
            Assert.True(dtb.Wert);
            b = dtb.WertNachBytes();
            Assert.True(b.Count() == 1);
            Assert.True(b[0] == 1);

            dtb.WertAusString("false");
            Assert.False(dtb.Wert);
            Assert.True(dtb.WertSerialisieren() == "False");
            Assert.True(dtb == false);

            dtb.WertAusString("true");
            Assert.True(dtb.Wert);
            Assert.True(dtb.WertSerialisieren() == "True");
            Assert.True(dtb == true);

            var dm = new isci.Daten.Datenmodell("Testdatenmodell");
            dm.Add(dtb);
            Assert.True(dtb.Identifikation == "Testdatenmodell.bool");

            var dtb_ = Dateneintrag.GibDateneintragTypisiert(Newtonsoft.Json.Linq.JObject.Parse(dtb.DateneintragSerialisiert()));
            Assert.True(dtb_.Identifikation == "Testdatenmodell.bool");
            Assert.True(dtb_.type == Datentypen.Bool);
            Assert.True(((isci.Daten.dtBool)dtb_).Wert == true);
            Assert.Throws<Exception>(() => dm.Add(dtb_));

            output.WriteLine(dtb.Identifikation);            

            var struktur = new isci.Daten.Datenstruktur("Datenstruktur", "Testmodul", "Testautomatisierungssystem");
            struktur.DatenmodellEinhängen(dm);
            Assert.Contains("Testdatenmodell", struktur.datenmodelle);
            output.WriteLine(dtb.Identifikation);
            Assert.True(struktur.dateneinträge.ContainsKey("Testautomatisierungssystem.Testdatenmodell.bool"));
            Assert.True(struktur["Testautomatisierungssystem.Testdatenmodell.bool"].Identifikation == "Testautomatisierungssystem.Testdatenmodell.bool");
            Assert.True(struktur["Testdatenmodell.bool"].Identifikation == "Testautomatisierungssystem.Testdatenmodell.bool");


            Assert.True((isci.Daten.dtBool)struktur["Testautomatisierungssystem.Testdatenmodell.bool"] == true);
            Assert.True((isci.Daten.dtBool)struktur["Testdatenmodell.bool"] == true);
            struktur.Start();

            dtb.Wert = false;
            dtb.WertInSpeicherSchreiben();

            var stream = new System.IO.FileStream("Datenstruktur/Testautomatisierungssystem/Testautomatisierungssystem.Testdatenmodell.bool", System.IO.FileMode.Open);
            var reader = new System.IO.BinaryReader(stream);

            Assert.False(reader.ReadBoolean());


            dtb.Wert = true;
            dtb.WertInSpeicherSchreiben();

            reader.Close();
            stream.Close();

            stream = new System.IO.FileStream("Datenstruktur/Testautomatisierungssystem/Testautomatisierungssystem.Testdatenmodell.bool", System.IO.FileMode.Open);
            reader = new System.IO.BinaryReader(stream);

            Assert.True(reader.ReadBoolean());

            reader.Close();
            stream.Close();

            var bool_als_token = new Newtonsoft.Json.Linq.JValue(false);
            dtb.WertAusJToken(bool_als_token);
            Assert.False(dtb.Wert);

            bool_als_token = new Newtonsoft.Json.Linq.JValue(true);
            dtb.WertAusJToken(bool_als_token);
            Assert.True(dtb.Wert);

            Assert.True(dtb.DateneintragSerialisiert() == "{\"Identifikation\":\"Testautomatisierungssystem.Testdatenmodell.bool\",\"type\":\"Bool\",\"istListe\":false,\"listeDimensionen\":0,\"parentEintrag\":null,\"Einheit\":-1,\"Wert\":true}");

            




            Console.WriteLine(dtb.DateneintragSerialisiert());

        }

        [Fact]
        public void Test_Datenmodell()
        {
        }

        [Fact]
        public void Test_Datenstruktur()
        {
        }
    }
}