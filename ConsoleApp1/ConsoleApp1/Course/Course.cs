using System;
using System.Collections.Generic;

namespace CourseChecker.Course
{
    public class Kurse
    {
        private String strKursNr;
        private String strKursTitel;
        private DateTime dateBeginn;

        private DateTime dateEnde;
        private String strOrt;
        private int iPreis;
        private Boolean boolGarantieTermin;
        private String strReason;
        private static readonly Dictionary<String, String> dictReason = new Dictionary<string, string>() {
            { "Preis", "Kurspreis wurde geändert;"},
            { "Garantie", "Kurs ist Garantietermin;" },
            { "!Garantie", "Kurs ist nicht mehr Garantietermin;" },
            { "!Kurs", "Kurs entfernt;" },
            { "Kurs", "Kurs hinzugefügt;" }
        };

        
        public Kurse(String strKursNr, String strKursTitel, DateTime dateBeginn, DateTime dateEnde, String strOrt, int iPreis)
        {
            this.strKursNr = strKursNr;
            this.strKursTitel = strKursTitel;
            this.dateBeginn = dateBeginn;
            this.dateEnde = dateEnde;
            this.strOrt = strOrt;
            this.iPreis = iPreis;
            this.boolGarantieTermin = false;
            this.strReason = "";
        }

        public Kurse(String strKursNr, String strKursTitel, DateTime dateBeginn, DateTime dateEnde, String strOrt, int iPreis, Boolean boolGarantieTermin)
        {
            this.strKursNr = strKursNr;
            this.strKursTitel = strKursTitel;
            this.dateBeginn = dateBeginn;
            this.dateEnde = dateEnde;
            this.strOrt = strOrt;
            this.iPreis = iPreis;
            this.boolGarantieTermin = boolGarantieTermin;
            this.strReason = "";
        }

        public String StrOrt => strOrt;
        public int IPreis => iPreis;
        public Boolean BoolGarantieTermin { get { return this.boolGarantieTermin; } set { this.boolGarantieTermin = value; } }
        public DateTime DateEnde => dateEnde;
        public DateTime DateBeginn => dateBeginn;
        public String StrKursTitel => strKursTitel;
        public String StrKursNr => strKursNr;
        public String StrReason => strReason;

        public Boolean Contains(Kurse kursCheck)
        {
            Boolean retEqual = false;
            if (this.strKursNr.Length > 0)
                retEqual = this.strKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant())
                        & this.dateBeginn.Equals(kursCheck.DateBeginn)
                        & this.dateEnde.Equals(kursCheck.DateEnde)
                        & this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())
                        & this.iPreis.Equals(kursCheck.IPreis)
                        & this.boolGarantieTermin.Equals(kursCheck.BoolGarantieTermin);
            else
                retEqual = this.strKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant())
                        & this.dateBeginn.Equals(kursCheck.DateBeginn)
                        & this.dateEnde.Equals(kursCheck.DateEnde)
                        & this.iPreis.Equals(kursCheck.IPreis)
                        & this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())
                        & this.boolGarantieTermin.Equals(kursCheck.BoolGarantieTermin)
                        & this.strKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim().Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim());

            if (!retEqual && this.strKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim().Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim()) && this.dateBeginn.Equals(kursCheck.DateBeginn) && this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())) {
                if (!this.iPreis.Equals(kursCheck.IPreis))
                    this.strReason = dictReason["Preis"];
                if(!this.boolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && this.boolGarantieTermin)
                    this.strReason = dictReason["Garantie"];
                if (!this.boolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && kursCheck.BoolGarantieTermin)
                    this.strReason = dictReason["!Garantie"];
            }else if (!retEqual && !(this.strReason.Length > 1)) {
                this.strReason = dictReason["Kurs"];
            }

            return retEqual;
        }

        public Boolean ContainsForIDS(Kurse kursCheck)
        {
            Boolean retEqual = false;
            if (this.strKursNr.Length > 0)
                retEqual = this.strKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant())
                        & this.dateBeginn.Equals(kursCheck.DateBeginn)
                        & this.dateEnde.Equals(kursCheck.DateEnde)
                        & this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant());
            else
                retEqual = this.strKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant())
                        & this.dateBeginn.Equals(kursCheck.DateBeginn)
                        & this.dateEnde.Equals(kursCheck.DateEnde)
                        & this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())
                        & this.strKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim().Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim());


            if (!retEqual && this.strKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim().Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Replace(":", "").Trim()) && this.dateBeginn.Equals(kursCheck.DateBeginn) && this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant())) {
                if (!this.iPreis.Equals(kursCheck.IPreis) && this.strKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant()))
                    kursCheck.strReason = dictReason["Preis"];
                if (!this.boolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && this.boolGarantieTermin)
                    kursCheck.strReason = dictReason["Garantie"];
                if (!this.boolGarantieTermin.Equals(kursCheck.BoolGarantieTermin) && kursCheck.BoolGarantieTermin)
                    kursCheck.strReason = dictReason["!Garantie"];
            }else if (!retEqual) {
                kursCheck.strReason = dictReason["!Kurs"];
            }

            return retEqual;
        }

    }
}
