# 🏥 BDD-Testkurs für MedTech — Reqnroll + Playwright + Blazor
### Gherkin · C# · In-Memory DB · 1,5 Wochen Intensivkurs

> **Kontext:** Du arbeitest in einem MedTech-Unternehmen, das Software für Ärzte entwickelt (Blazor-Frontend). Du lernst BDD-Tests mit **Reqnroll** (dem Community-Fork von SpecFlow, Open Source), **Playwright** für Browser-Tests und **EF Core InMemory** für isolierte Datenbanktests.

---

## ⚠️ Ehrliche Einschätzung zum Lernplan

Dieser Kurs deckt in 1,5 Wochen ein Themenspektrum ab, das normalerweise 4–6 Wochen füllt. Das Ziel ist nicht, alles perfekt zu beherrschen — sondern **handlungsfähig genug zu sein**, um am ersten Arbeitstag kompetent mitzuwirken und Verständnis für die Architektur zu zeigen.

**Empfohlene Prioritäten:**

```
Woche 1 (Tage 1–5): MUSS beherrscht werden
→ Gherkin · Step Definitions · Hooks · InMemory DB

Woche 1.5 (Tage 6–11): Überblick reicht für den Start
→ Playwright · E2E · API-Tests · CI/CD
```

---

## 🏗️ Test-Pyramide — Grundprinzip für diesen Kurs

Bevor wir anfangen: Verstehe die **Test-Pyramide**. Sie bestimmt, wie viele Tests auf welcher Ebene geschrieben werden.

```
        🔴  E2E / Browser-Tests (Playwright)        ← ~10%
           Langsam · fragil · teuer · selten

      🟡    Integrationstests (EF Core / API)        ← ~20%
           Mittel schnell · realistischer als Unit

   🟢        Unit-Tests (Business Logic / Services)  ← ~70%
             Schnell · stabil · günstig · viele
```

**Konkret für MedTech:**

In einem Rezept-Feature bedeutet das:

🟢 **70% Unit-Tests** — `RezeptService.VerschreibeMedikament()` prüfen: Allergie-Logik, Wechselwirkungen, Dosierungsgrenzen — rein in C#, ohne DB, ohne Browser.

🟡 **20% Integrationstests** — Wird das Rezept korrekt in der InMemory-DB gespeichert? Gibt die API die richtigen Status-Codes zurück?

🔴 **10% E2E-Tests** — Nur der kritische Happy Path im Browser: Arzt verschreibt Medikament, PDF wird generiert, Audit-Log erscheint.

> **Faustformel:** Wenn du versucht bist, alles in E2E-Tests zu prüfen — schreib stattdessen einen Unit-Test. Er ist 100× schneller und robuster.

---

## 📅 Lernplan

| Tag | Thema | Schwerpunkt | Dauer |
|-----|-------|-------------|-------|
| Tag 1A | BDD-Grundidee im MedTech-Kontext | 🟢 Einstieg | 60–90 Min. |
| Tag 1B | Reqnroll, Projektaufbau und reqnroll.json | 🟢 Setup | 60–90 Min. |
| Tag 1C | Gherkin auf Deutsch lesen und schreiben | 🟢 Praxis | 60–90 Min. |
| Tag 2 | Step Definitions in C# | 🟢 Unit-Ebene | 3–4 Std. |
| Tag 3 | Hooks, Backgrounds, Tags | 🟢 Struktur | 3 Std. |
| Tag 4 | Scenario Outline & Data Tables | 🟢 Parametrisierung | 3 Std. |
| Tag 5 | EF Core InMemory DB | 🟡 Integration | 3–4 Std. |
| Tag 6 | Playwright + Blazor UI-Tests | 🔴 E2E | 4 Std. |
| Tag 7 | **Mini-Projekt 1:** Patientenakte (Unit + Integration) | 🟢🟡 | ganzer Tag |
| Tag 8 | API-Testing (REST) | 🟡 Integration | 3–4 Std. |
| Tag 9 | Reporting, CI/CD | ⚙️ DevOps | 3 Std. |
| Tag 10 | **Mini-Projekt 2:** E2E Rezept-Workflow | 🔴 E2E | ganzer Tag |
| Tag 11 | Best Practices, Refactoring | 🧠 Review | 3 Std. |

---

## 📘 TAG 1A — BDD-Grundidee im MedTech-Kontext

### 1A.0 Was du heute lernst und warum

Bevor du Tools oder Syntax lernst, brauchst du ein klares Bild davon, **warum** BDD überhaupt eingesetzt wird. An diesem Tag geht es darum, die Denkweise hinter dem Ansatz zu verstehen: Du beschreibst nicht primär Code, sondern fachliches Verhalten, das für Ärzte, QM und Entwicklung gemeinsam verständlich ist.

Das ist wichtig, weil gute BDD-Tests in MedTech nicht nur Fehler finden sollen. Sie dienen auch als gemeinsame Sprache zwischen Fachseite und Technik und helfen dabei, regulatorisch relevante Anforderungen nachvollziehbar zu dokumentieren.

### 1A.1 Was ist BDD überhaupt?

**BDD (Behavior-Driven Development)** bedeutet: Du beschreibst das erwartete Verhalten eines Systems aus Sicht der Fachseite, also aus Sicht von Arzt, Pflegekraft, Patient oder QM. Du beschreibst **nicht zuerst die technische Implementierung**.

Kurz gesagt:

- Ein klassischer Unit-Test prüft Methoden und Rückgabewerte.
- Ein BDD-Szenario beschreibt fachliches Verhalten in verständlicher Sprache.
- Beide sind nützlich, aber sie beantworten unterschiedliche Fragen.

```text
Technischer Test:                     Fachliches BDD-Szenario:
[Test]                               Szenario: Arzt sucht Patient
FindById(42) liefert Datensatz       Angenommen Dr. Müller ist eingeloggt
                                     Wenn er nach "Schmidt" sucht
                                     Dann sollte "Hans Schmidt" erscheinen
```

### 1A.2 Warum ist BDD in MedTech sinnvoll?

Im MedTech-Umfeld ist BDD besonders hilfreich, weil Tests nicht nur für Entwickler geschrieben werden.

- Regulatorische Anforderungen wie MDR oder FDA 21 CFR Part 11 verlangen nachvollziehbare Tests.
- Feature-Dateien können als **lebende Dokumentation** für Audits dienen.
- Fachpersonen wie Ärzte oder QM-Manager können das erwartete Verhalten mitlesen und validieren.

### 1A.3 Woran erkennst du ein gutes BDD-Szenario?

Ein gutes Szenario ist fachlich, konkret und knapp.

**Schlecht:**

```gherkin
Wenn ich auf den Button mit der ID "save-btn" klicke
```

**Besser:**

```gherkin
Wenn der Arzt das Rezept speichert
```

Der Unterschied ist wichtig: Die zweite Formulierung beschreibt Verhalten. Die erste beschreibt UI-Details, die sich jederzeit ändern können.

### 1A.4 Mini-Merkregel

Wenn dein Schritt eher nach HTML, CSS, XPath oder API-Endpunkt klingt, ist er meist zu technisch. Wenn dein Schritt wie ein Satz aus dem Arbeitsalltag einer Praxis klingt, bist du näher an gutem BDD.

### 1A.5 Tageszusammenfassung

Heute hast du verstanden, was BDD im Kern bedeutet: fachliches Verhalten so zu beschreiben, dass es für technische und nicht-technische Beteiligte lesbar bleibt.

Du hast außerdem gesehen, warum das gerade im MedTech-Kontext wichtig ist: Tests sind hier nicht nur Qualitätswerkzeug, sondern oft auch Teil von Nachvollziehbarkeit, Kommunikation und Audit-Vorbereitung.

---

## 📘 TAG 1B — Reqnroll, Projektaufbau und reqnroll.json

### 1B.0 Was du heute lernst und warum

Heute klärst du die technische Grundlage des Kurses. Du lernst, welche Rolle Reqnroll im Test-Stack übernimmt, wie das Projekt organisiert ist und warum die Konfiguration in `reqnroll.json` so wichtig für Sprache, Parsing und Debugging ist.

Das Ziel dieses Tages ist nicht, sofort alles auswendig zu können. Du sollst verstehen, **welcher Baustein wofür zuständig ist**, damit die spätere Arbeit mit Features, Steps und Hooks nicht wie einzelne lose Fragmente wirkt.

### 1B.1 Was ist Reqnroll?

**Reqnroll** ist ein aktiv gepflegter **Open-Source-Fork von SpecFlow**. Die Syntax ist weitgehend kompatibel, das Projekt ist kostenlos nutzbar und für moderne .NET-Projekte gut geeignet.

```text
SpecFlow (historisch bekannt, später stärker kommerzialisiert)
        ↓ Community-Fork
Reqnroll (Open Source, aktiv gepflegt, für .NET 8+ geeignet)
```

Für das Verständnis am Anfang reicht:

- `.feature`-Dateien beschreiben Verhalten.
- Step Definitions verbinden diese Sätze mit C#-Code.
- Hooks bereiten Testzustand vor oder räumen auf.
- `reqnroll.json` steuert das Verhalten von Reqnroll.

### 1B.2 Projektstruktur verstehen

```text
MedTech.Tests/
├── Features/              -> fachliche Szenarien in Gherkin
├── StepDefinitions/       -> C#-Methoden zu Given/When/Then
├── Hooks/                 -> Setup und Teardown pro Szenario
├── Pages/                 -> Page Objects für UI-Tests mit Playwright
├── Infrastructure/        -> Test-Helfer, DB-Kontext, Services
└── reqnroll.json          -> Reqnroll-Konfiguration
```

**Merke:**

- `Features/` ist für Fachsprache.
- `StepDefinitions/` ist die Übersetzung in Code.
- `Infrastructure/` enthält technische Hilfen, die du nicht in jedes Szenario schreiben willst.

### 1B.3 Welche Pakete brauchst du am Anfang wirklich?

Das Projekt enthält alle Pakete, die im Verlauf des Kurses gebraucht werden. Du musst sie nicht alle sofort verstehen — merke dir, **wann** jedes Paket relevant wird:

| Paket | Ab Tag | Zweck |
|-------|--------|-------|
| `Reqnroll` + `Reqnroll.NUnit` | Tag 1 | BDD-Framework, Kernkomponente |
| `NUnit`, `NUnit3TestAdapter`, `Microsoft.NET.Test.Sdk` | Tag 1 | Test-Runner |
| `FluentAssertions` | Tag 2 | Lesbare Assertions (`.Should().Be()`) |
| `Microsoft.EntityFrameworkCore.InMemory` | Tag 5 | InMemory-Datenbank für isolierte Tests |
| `RestSharp`, `Newtonsoft.Json` | Tag 8 | REST-API-Tests |
| `Microsoft.AspNetCore.Mvc.Testing` | Tag 8 | Blazor-App lokal als Test-Host starten |
| `Microsoft.Playwright` | Tag 6 | Browser-Automatisierung |

```xml
<!-- MedTech.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <!-- ✅ Tag 1–2: BDD-Kern + Test-Runner + Assertions -->
    <PackageReference Include="Reqnroll" Version="2.1.0" />
    <PackageReference Include="Reqnroll.NUnit" Version="2.1.0" />
    <PackageReference Include="NUnit" Version="4.1.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />

    <!-- ✅ Tag 5: InMemory-Datenbank -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />

    <!-- ✅ Tag 8: API-Tests + lokaler Test-Host -->
    <PackageReference Include="RestSharp" Version="110.2.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />

    <!-- ✅ Tag 6: Browser-Tests -->
    <PackageReference Include="Microsoft.Playwright" Version="1.42.0" />
    <PackageReference Include="Microsoft.Playwright.NUnit" Version="1.42.0" />
  </ItemGroup>
</Project>
```

### 1B.4 reqnroll.json Schritt für Schritt verstehen

Die Datei `reqnroll.json` ist die zentrale Konfigurationsdatei für Reqnroll. Sie bestimmt unter anderem:

- in welcher Sprache deine `.feature`-Dateien geschrieben werden,
- wie Zahlen geparst werden,
- und wie ausführlich Reqnroll in der Konsole loggt.

```json
{
  "$schema": "https://schemas.reqnroll.net/reqnroll-config-latest.json",
  "language": {
    "feature": "de-DE"
  },
  "bindingCulture": {
    "name": "de-DE"
  },
  "trace": {
    "traceSuccessfulSteps": true
  }
}
```

**Was bedeutet jede Zeile?**

| Schlüssel | Bedeutung | Praktischer Effekt |
|-----------|-----------|--------------------|
| `language.feature` | Sprache der Gherkin-Schlüsselwörter | Du kannst `Funktionalität`, `Szenario`, `Angenommen`, `Wenn`, `Dann` verwenden |
| `bindingCulture.name` | Kultur für Zahlen und Datumswerte | `1,5` wird in deutscher Kultur korrekt geparst |
| `trace.traceSuccessfulSteps` | Loggt auch erfolgreiche Steps | Hilfreich beim Lernen und Debuggen |

> **Wichtig:** Wenn du deutsche Feature-Dateien schreibst, sollte meistens auch `bindingCulture` auf `de-DE` stehen. Sonst können Zahlenformate wie `1,5` zu Parsefehlern führen.

Die oben gezeigte Konfiguration deckt alle drei Aspekte ab und ist die empfohlene Startkonfiguration für diesen Kurs.

### 1B.5 Tageszusammenfassung

Heute hast du die technische Basis des Kurses aufgebaut: Reqnroll als BDD-Framework eingeordnet, die Ordnerstruktur verstanden und die wichtigsten Pakete einem Zweck zugeordnet.

Außerdem ist klar, warum `reqnroll.json` zentral ist: Die Datei steuert Sprache, Kultur und Logging und sorgt dafür, dass deine Feature-Dateien und deine Testausführung zusammenpassen.

---

## 📘 TAG 1C — Gherkin auf Deutsch lesen und schreiben

### 1C.0 Was du heute lernst und warum

Heute geht es um die Sprache von BDD: **Gherkin**. Du lernst, wie Features, Grundlagen und Szenarien aufgebaut sind und wie du fachliche Anforderungen so formulierst, dass sie für Menschen verständlich bleiben und später trotzdem automatisiert werden können.

Das ist entscheidend, weil schlechte Gherkin-Szenarien häufig zu technischen, schwer wartbaren Tests führen. Gute Gherkin-Texte helfen dir dagegen, Verhalten sauber zu schneiden, Wiederverwendung zu fördern und Diskussionen mit Fachbereichen zu vereinfachen.

### 1C.1 Ein minimales Feature lesen

```gherkin
Funktionalität: Patientenakte einsehen
  Als behandelnder Arzt
  möchte ich die Patientenakte öffnen können
  damit ich die medizinische Geschichte des Patienten kenne

  Grundlage:
    Angenommen der Arzt ist eingeloggt

  Szenario: Akte eines vorhandenen Patienten öffnen
    Wenn er nach Patient "Hans Schmidt" sucht
    Dann sollte die Akte geöffnet werden
    Und die medizinische Vorgeschichte sollte sichtbar sein
```

### 1C.2 Wie liest man so ein Feature?

Du kannst ein Feature immer in drei Ebenen lesen:

1. `Funktionalität:` beschreibt das übergeordnete Thema.
2. `Als / möchte ich / damit` beschreibt den fachlichen Nutzen.
3. `Szenario` beschreibt einen konkreten Testfall.

### 1C.3 Die wichtigsten Gherkin-Schlüsselwörter auf Deutsch

| Englisch | Deutsch | Bedeutung |
|----------|---------|-----------|
| `Feature:` | `Funktionalität:` | Übergeordnetes Thema |
| `Scenario:` | `Szenario:` | Ein einzelner Testfall |
| `Given` | `Angenommen` / `Gegeben` | Ausgangszustand |
| `When` | `Wenn` | Aktion |
| `Then` | `Dann` | Erwartetes Ergebnis |
| `And` | `Und` | Zusätzlicher Schritt |
| `But` | `Aber` | Ausnahme oder Einschränkung |
| `Background:` | `Grundlage:` | Gemeinsame Vorbedingung |
| `Scenario Outline:` | `Szenariogrundriss:` | Parametrisierter Test |
| `Examples:` | `Beispiele:` | Beispieldaten |

### 1C.4 Erstes vollständiges Beispiel auf Deutsch

```gherkin
Funktionalität: Medikamentenverschreibung
  Als behandelnder Arzt
  möchte ich Medikamente sicher verschreiben
  damit Patienten die richtige Behandlung ohne gefährliche Wechselwirkungen erhalten

  Grundlage:
    Angenommen Dr. Weber ist im System eingeloggt
    Und die Akte von Patientin "Maria Hoffmann" mit der ID "P-4421" ist geöffnet

  @smoke @positiv
  Szenario: Erfolgreiche Medikamentenverschreibung
    Wenn Dr. Weber "Ibuprofen 400 mg" zweimal täglich verschreibt
    Dann sollte das Rezept gespeichert werden
    Und das Medikament sollte in der Liste der aktiven Medikamente erscheinen
    Und ein Rezept-PDF sollte erzeugt werden

  @sicherheit @allergie
  Szenario: Warnung bei bekannter Allergie
    Angenommen Patientin "Maria Hoffmann" hat eine dokumentierte Allergie gegen "Penicillin"
    Wenn Dr. Weber versucht, "Amoxicillin" zu verschreiben
    Dann sollte eine rote Allergiewarnung erscheinen: "Patientin reagiert allergisch auf Penicillin-Antibiotika"
    Und das Rezept sollte NICHT automatisch gespeichert werden
    Und Dr. Weber muss eine Begründung für das Überschreiben angeben

  @sicherheit @wechselwirkung
  Szenario: Warnung bei kritischer Wechselwirkung
    Angenommen Patientin "Maria Hoffmann" nimmt aktuell "Warfarin" ein
    Wenn Dr. Weber "Aspirin 100 mg" verschreibt
    Dann sollte eine Wechselwirkungswarnung erscheinen: "Erhöhtes Blutungsrisiko bei Kombination mit Warfarin"
    Und der Schweregrad sollte als "HOCH" markiert sein
    Und das System sollte die Alternative "Paracetamol" vorschlagen
```

### 1C.5 Übung 1.1 — Erstes eigenes Feature schreiben

Schreibe nun selbst ein `.feature`-File für die **Medikamentenverschreibung**.

- Szenario 1: Arzt verschreibt ein Medikament erfolgreich.
- Szenario 2: Warnung bei bekannter Allergie des Patienten.
- Szenario 3: Warnung bei gefährlicher Wechselwirkung mit bestehendem Medikament.

Wenn du nicht weiterkommst, orientiere dich an diesem Muster:

<details>
<summary>💡 Musterlösung</summary>

```gherkin
Funktionalität: Medikamentenverschreibung
  Als behandelnder Arzt
  möchte ich Medikamente sicher verschreiben
  damit Patienten ohne vermeidbare Risiken behandelt werden

  Grundlage:
    Angenommen Dr. Weber ist im System eingeloggt
    Und die Akte von Patientin "Maria Hoffmann" ist geöffnet

  Szenario: Erfolgreiche Verschreibung
    Wenn Dr. Weber "Ibuprofen 400 mg" verschreibt
    Dann sollte das Rezept gespeichert werden

  Szenario: Allergiewarnung bei Penicillin
    Angenommen Patientin "Maria Hoffmann" hat eine Allergie gegen "Penicillin"
    Wenn Dr. Weber "Amoxicillin" verschreibt
    Dann sollte eine Allergiewarnung erscheinen
    Und das Rezept sollte nicht automatisch gespeichert werden

  Szenario: Wechselwirkungswarnung bei Warfarin
    Angenommen Patientin "Maria Hoffmann" nimmt "Warfarin" ein
    Wenn Dr. Weber "Aspirin 100 mg" verschreibt
    Dann sollte eine Wechselwirkungswarnung erscheinen
```
</details>

### 1C.6 Tageszusammenfassung

Heute hast du gelernt, Gherkin-Strukturen zu lesen und selbst erste fachlich formulierte Szenarien zu schreiben.

Wichtig ist dabei der Unterschied zwischen Fachsprache und Implementierungsdetails: Ein gutes Szenario beschreibt, **was** der Arzt oder das System tun soll, nicht **welchen Button** oder **welchen Selektor** der Test intern verwendet.

---

## 📘 TAG 2 — Step Definitions in C#

### 2.0 Was du heute lernst und warum

Heute verbindest du Fachsprache mit ausführbarem Code. **Step Definitions** sind die Übersetzungsschicht zwischen den Gherkin-Sätzen in der Feature-Datei und dem C#-Code, der tatsächlich Testdaten anlegt, Services aufruft und Ergebnisse prüft.

Das ist einer der wichtigsten Tage des Kurses, weil hier aus Text echte Automatisierung wird. Du lernst außerdem, warum sauber benannte, wiederverwendbare Steps langfristig viel wertvoller sind als viele fast gleiche Speziallösungen.

> **Hinweis zur Sprache:** Die Step-Texte in den folgenden Code-Beispielen sind auf Englisch (`"Dr. Weber is logged into the system"`), obwohl `reqnroll.json` auf `de-DE` konfiguriert ist. Das ist kein Widerspruch: `language.feature` steuert nur die Gherkin-**Schlüsselwörter** (`Angenommen`, `Wenn`, `Dann`) — nicht den frei formulierten Schritttext. Ob dein Team englische oder deutsche Schritttexte verwendet, ist eine Teamentscheidung. Wichtig ist nur **Konsistenz im Projekt**.

### 2.1 Grundaufbau

```csharp
// StepDefinitions/RezeptSteps.cs
using Reqnroll;
using NUnit.Framework;
using FluentAssertions;
using MedTech.Tests.Infrastructure;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class RezeptSteps
{
    // Reqnroll's Dependency Injection
    private readonly ScenarioContext _context;
    private readonly TestDbContext _db;

    // Lokaler State des Szenarios
    private string _warnung = string.Empty;
    private bool _rezeptGespeichert = false;

    public RezeptSteps(ScenarioContext context, TestDbContext db)
    {
        _context = context;
        _db = db;
    }

    // -------------------------------------------------------
    // Hinweis: _context["Schlüssel"] = wert ist der einfachste
    // Einstieg, aber untypisiert (String-Keys, mögliche
    // Laufzeitfehler). In komplexeren Projekten besser eine
    // typisierte Kontextklasse nutzen → siehe Tag 11.3.
    // -------------------------------------------------------

    [Given(@"Dr\. Weber is logged into the system")]
    public void GegebenDrWeberIstEingeloggt()
    {
        // Arzt in InMemory-DB suchen/anlegen
        var arzt = _db.Aerzte.FirstOrDefault(a => a.Name == "Dr. Weber")
                   ?? new Arzt { Name = "Dr. Weber", Fachrichtung = "Allgemeinmedizin" };

        _db.Aerzte.Add(arzt);
        _db.SaveChanges();

        _context["AktuellerArzt"] = arzt;
    }

    [Given(@"patient ""(.*)"" \(ID: (.*)\) is open")]
    public void GegebenPatientIstGeoeffnet(string name, string patientId)
    {
        var patient = new Patient
        {
            PatientId = patientId,
            Name = name,
            Allergien = new List<string>(),
            AktiveMedikamente = new List<string>()
        };

        _db.Patienten.Add(patient);
        _db.SaveChanges();

        _context["AktuellerPatient"] = patient;
    }

    [Given(@"patient ""(.*)"" has a documented allergy to ""(.*)""")]
    public void GegebenPatientHatAllergie(string patientName, string allergen)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        patient.Allergien.Add(allergen);
        _db.Patienten.Update(patient); // Explizit markieren, da List<string> per HasConversion gespeichert wird
        _db.SaveChanges();
    }

    [Given(@"patient ""(.*)"" is currently taking ""(.*)""")]
    public void GegebenPatientNimmtMedikament(string patientName, string medikament)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        patient.AktiveMedikamente.Add(medikament);
        _db.SaveChanges();
    }

    [When(@"Dr\. Weber prescribes ""(.*)"" twice daily")]
    public void WennDrWeberVerschreibt(string medikament)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        var arzt = _context.Get<Arzt>("AktuellerArzt");

        // Business-Logik (Service aufrufen)
        var rezeptService = new RezeptService(_db);
        var ergebnis = rezeptService.VerschreibeMedikament(patient, arzt, medikament, "2x täglich");

        _context["Ergebnis"] = ergebnis;
        _rezeptGespeichert = ergebnis.ErfolgreichGespeichert;
        _warnung = ergebnis.Warnung ?? string.Empty;
    }

    [When(@"Dr\. Weber tries to prescribe ""(.*)""")]
    public void WennDrWeberVersuchtZuVerschreiben(string medikament)
    {
        WennDrWeberVerschreibt(medikament);
    }

    [Then(@"the prescription should be saved")]
    public void DannSollteRezeptGespeichertSein()
    {
        _rezeptGespeichert.Should().BeTrue("Das Rezept sollte gespeichert worden sein");
    }

    [Then(@"the prescription should NOT be saved automatically")]
    public void DannSollteRezeptNichtAutomatischGespeichertSein()
    {
        _rezeptGespeichert.Should().BeFalse("Bei Allergie darf kein Rezept automatisch gespeichert werden");
    }

    [Then(@"a red allergy warning should appear: ""(.*)""")]
    public void DannSollteAllergieWarnungErscheinen(string erwartet)
    {
        _warnung.Should().Contain(erwartet,
            "Die Allergie-Warnung muss für den Arzt klar sichtbar sein");
    }

    [Then(@"the severity should be marked as ""(.*)""")]
    public void DannSollteSchweregradusMarkiertSein(string erwartet)
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.WarnungSchweregrad.Should().Be(erwartet);
    }

    [Then(@"the system should suggest ""(.*)""")]
    public void DannSollteSystemVorschlagen(string erwartet)
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.Vorschlag.Should().Be(erwartet);
    }

    [Then(@"the medication should appear in the patient's active medications")]
    public void DannSollteMedikamentInAktivenMedikamentenErscheinen()
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        var aktualisiertPatient = _db.Patienten.Find(patient.Id);
        aktualisiertPatient!.AktiveMedikamente.Should().NotBeEmpty();
    }

    [Then(@"a prescription PDF should be generated")]
    public void DannSolltePdfGeneriertWerden()
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.PdfPfad.Should().NotBeNullOrEmpty("Es muss ein druckbares PDF geben");
    }
}
```

### 2.2 Cucumber Expressions vs. Regex

```csharp
// ❌ Regex (funktioniert, aber schwerer lesbar)
[Given(@"patient ""(.*)"" has (\d+) active prescriptions")]
public void PatientHatRezepte(string name, int anzahl) { }

// ✅ Cucumber Expressions (moderner, empfohlen)
[Given("patient {string} has {int} active prescriptions")]
public void PatientHatRezepte(string name, int anzahl) { }

// Verfügbare Typen:
// {string}  → "Text in Anführungszeichen"
// {int}     → ganze Zahlen: 42
// {float}   → Dezimalzahlen: 3.5
// {word}    → einzelnes Wort: HIGH
// {double}  → Gleitkomma: 98.6
```

### 🏋️ Übung 2.1

Implementiere Step Definitions für folgendes Szenario:

```gherkin
Scenario: Doctor views patient's blood pressure history
  Given patient "Klaus Bauer" has blood pressure readings:
    | Date       | Systolic | Diastolic |
    | 2024-01-10 | 145      | 92        |
    | 2024-01-17 | 138      | 88        |
    | 2024-01-24 | 150      | 95        |
  When the doctor opens the blood pressure chart
  Then the average systolic should be 144
  And the trend should be marked as "RISING"
  And a clinical alert should be shown: "Hypertension Stage 1"
```

### 2.3 Debugging — Wenn ein Step nicht gefunden wird

Eine der häufigsten Anfängerhürden: Reqnroll findet keine passende Step Definition für einen Gherkin-Schritt. Die Fehlermeldung sieht so aus:

```
No matching step definition found for the step "Dr. Weber ist im System eingeloggt"
```

**Warum passiert das?**

- Tippfehler zwischen Feature-Text und `[Given]`-Muster
- Regex-Sonderzeichen nicht escaped (Punkt in `Dr.` → muss `Dr\.` heißen)
- `[Binding]`-Attribut fehlt an der Klasse
- Step-Klasse liegt in einem anderen Assembly als das Testprojekt

**Reqnroll generiert automatisch einen Code-Stub**, den du als Ausgangspunkt kopieren kannst:

```csharp
// Generierter Stub in der Konsolenausgabe:
[Given(@"Dr\. Weber ist im System eingeloggt")]
public void GegebenDrWeberIstImSystemEingeloggt()
{
    throw new PendingStepException();
}
```

**Checkliste bei "No matching step definition":**

```
□ Stimmt der Text in .feature exakt mit dem Muster in [Given/When/Then] überein?
□ Ist die Klasse mit [Binding] dekoriert?
□ Bei Regex: Sind Sonderzeichen escaped? (Dr\. statt Dr.)
□ Bei Cucumber Expressions: {string} für "Text in Anführungszeichen" nutzen
□ traceSuccessfulSteps: true gesetzt? → zeigt, welche Steps laufen und wo es stoppt
```

> **Tipp:** `dotnet test --logger "console;verbosity=detailed"` gibt alle ausgeführten Steps in der Konsole aus — hilfreich beim ersten Debugging.

### 2.4 Tageszusammenfassung

Heute hast du verstanden, welche Aufgabe Step Definitions im BDD-Stack haben: Sie binden Gherkin-Schritte an C#-Methoden und machen fachliche Szenarien testbar.

Außerdem hast du gesehen, warum Cucumber Expressions oft besser lesbar als Regex sind und warum ein klarer Szenariozustand entscheidend ist, damit Steps konsistent zusammenarbeiten.

---

## 📘 TAG 3 — Hooks, Backgrounds & Tags

### 3.0 Was du heute lernst und warum

Heute geht es um die Struktur um die Tests herum. **Hooks** sind vorbereitende oder nachgelagerte Aktionen, die automatisch vor oder nach Tests laufen, zum Beispiel Datenbanken initialisieren, Browser starten oder Fehler protokollieren. Du nutzt sie, um wiederkehrende technische Arbeit aus den Szenarien herauszuhalten.

**Backgrounds** beschreiben gemeinsame fachliche Vorbedingungen, die für mehrere Szenarien gelten. **Tags** sind Markierungen wie `@smoke`, `@browser` oder `@regulatorisch`. Damit kannst du Szenarien gruppieren, gezielt ausführen, Hooks nur für bestimmte Testarten aktivieren und Tests in CI/CD-Pipelines filtern.

Kurz gesagt: Heute lernst du, wie eine Test-Suite nicht nur funktioniert, sondern auch **organisiert, wartbar und steuerbar** wird.

### 3.1 Hooks

```csharp
// Hooks/DatabaseHooks.cs
using Reqnroll;
using Microsoft.EntityFrameworkCore;
using BoDi;

[Binding]
public class DatabaseHooks
{
    private readonly IObjectContainer _container;
    private readonly ScenarioContext _scenarioContext;

    public DatabaseHooks(IObjectContainer container, ScenarioContext context)
    {
        _container = container;
        _scenarioContext = context;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Console.WriteLine("🏥 MedTech Testlauf gestartet");
        Console.WriteLine($"⏰ Start: {DateTime.Now:dd.MM.yyyy HH:mm}");
    }

    // Vor JEDEM Szenario: frische InMemory-Datenbank
    [BeforeScenario(Order = 1)]
    public void InitialisiereDatenbank()
    {
        // WICHTIG: Jedes Szenario bekommt eine eigene, saubere DB
        // Datenbankname = Szenario-ID → garantiert Isolation
        var dbName = $"MedTechTest_{Guid.NewGuid()}";

        var optionen = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var dbContext = new TestDbContext(optionen);

        // Grunddaten (Lookup-Tabellen, Medikamentenliste) seeden
        dbContext.SeedTestData();

        // Im DI-Container registrieren → wird in Steps injiziert
        _container.RegisterInstanceAs<TestDbContext>(dbContext);

        Console.WriteLine($"🗄️ Frische InMemory-DB: {dbName}");
    }

    [BeforeScenario(Order = 2)]
    public void LoggeSzeanrio()
    {
        var titel = _scenarioContext.ScenarioInfo.Title;
        var tags = string.Join(", ", _scenarioContext.ScenarioInfo.Tags);
        Console.WriteLine($"▶️ Szenario: {titel}");
        Console.WriteLine($"🏷️ Tags: {tags}");
    }

    [AfterScenario]
    public void NachSzenario()
    {
        if (_scenarioContext.TestError != null)
        {
            Console.WriteLine($"❌ FEHLGESCHLAGEN: {_scenarioContext.TestError.Message}");
            // Screenshot würde hier gemacht (siehe PlaywrightHooks)
        }
        else
        {
            Console.WriteLine("✅ BESTANDEN");
        }

        // DB aufräumen
        var db = _container.Resolve<TestDbContext>();
        db?.Dispose();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Console.WriteLine("🏁 Testlauf abgeschlossen");
    }
}
```

### 3.2 Background — gemeinsame Vorbedingungen

Ein `Background` (`Grundlage:` auf Deutsch) enthält Schritte, die vor **jedem** Szenario in der Feature-Datei ausgeführt werden. Es ist das Gherkin-Äquivalent eines `[SetUp]` in NUnit — aber in fachlicher Sprache formuliert.

**Wann Background, wann Hook?**

| | `Grundlage:` (Background) | Hook (`[BeforeScenario]`) |
|--|--------------------------|--------------------------|
| Wo | `.feature`-Datei | C#-Klasse |
| Sprache | Gherkin (fachlich lesbar) | C# (technisch) |
| Sichtbar für Fachseite? | ✅ Ja | ❌ Nein |
| Geeignet für | Fachliche Vorbedingungen | Technisches Setup (DB, Browser) |

```gherkin
Funktionalität: Medikamentenverschreibung

  Grundlage:
    Angenommen Dr. Weber ist im System eingeloggt
    Und die Akte von Patientin "Maria Hoffmann" ist geöffnet

  Szenario: Erfolgreiche Verschreibung
    # Die beiden Grundlage-Steps laufen hier zuerst, dann dieser Step:
    Wenn Dr. Weber "Ibuprofen 400 mg" verschreibt
    Dann sollte das Rezept gespeichert werden

  Szenario: Allergiewarnung bei Penicillin
    # Die Grundlage-Steps laufen erneut — frischer, isolierter Zustand
    Angenommen Patientin "Maria Hoffmann" hat eine Allergie gegen "Penicillin"
    Wenn Dr. Weber "Amoxicillin" verschreibt
    Dann sollte eine Allergiewarnung erscheinen
```

> **Wichtig:** Der Background gilt für **alle** Szenarien der Datei. Wenn eine Vorbedingung nur für bestimmte Szenarien gilt, schreib sie direkt als `Angenommen`-Schritt ins jeweilige Szenario.

### 3.3 Tags in MedTech

```gherkin
@medikation @sicherheit @regression
Feature: Medikamentenverschreibung

  @smoke @kritisch @positiv
  Scenario: Arzt verschreibt Medikament erfolgreich
    ...

  @sicherheit @allergie @negativ
  Scenario: Allergie-Warnung erscheint
    ...

  @wip @in-entwicklung
  Scenario: KI-gestützte Dosierungsempfehlung (noch in Entwicklung)
    ...

  @regulatorisch @mdr @audit-relevant
  Scenario: Verschreibung wird im Audit-Log gespeichert
    ...
```

**Hooks für spezifische Tags:**

```csharp
// Nur für @browser-Tests (Playwright)
[BeforeScenario("browser", Order = 10)]
public async Task BrowserStarten()
{
    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new()
    {
        Headless = true // In CI immer headless
    });
    _container.RegisterInstanceAs(browser);
}

// Nur für @api-Tests
[BeforeScenario("api", Order = 5)]
public void ApiClientVorbereiten()
{
    var client = new RestClient("https://api.medtech-app.de");
    _container.RegisterInstanceAs(client);
}

// Für alle @audit-relevant Tests: Extra-Logging
[AfterScenario("audit-relevant")]
public void AuditLogPrüfen()
{
    var db = _container.Resolve<TestDbContext>();
    var auditEinträge = db.AuditLog.ToList();
    auditEinträge.Should().NotBeEmpty("Audit-Log muss gefüllt sein (MDR-Anforderung)");
}
```

**CLI-Filterung:**

```bash
# Nur Smoke-Tests
dotnet test --filter "Category=smoke"

# Nur sicherheitskritische Tests
dotnet test --filter "Category=sicherheit"

# Regulatorische Tests für Audit
dotnet test --filter "Category=regulatorisch"

# Alles außer wip
dotnet test --filter "Category!=wip"
```

### 3.4 Tageszusammenfassung

Heute hast du gelernt, dass Hooks technische Vor- und Nachbereitung automatisch übernehmen, damit deine Szenarien fachlich lesbar bleiben.

Du hast außerdem verstanden, dass Backgrounds gemeinsame fachliche Ausgangslagen beschreiben, während Hooks für technisches Setup (DB, Browser) zuständig sind. Tags dienen zur Organisation: für Testfilter, für unterschiedliche Testtypen und für spezielle Automatisierung in lokalen Läufen oder in CI/CD.

---

## 📘 TAG 4 — Scenario Outline & Data Tables

### 4.0 Was du heute lernst und warum

Heute geht es darum, wiederkehrende Testfälle mit unterschiedlichen Daten sauber zu modellieren. Ein **Scenario Outline** ist sinnvoll, wenn derselbe fachliche Ablauf mit mehreren Eingabewerten geprüft werden soll. **Data Tables** helfen dir, strukturierte Datensätze wie Laborwerte, Medikationslisten oder Vitaldaten lesbar direkt im Szenario zu erfassen.

Das ist wichtig, weil medizinische Software fast immer mit vielen Varianten und vielen Datenpunkten arbeitet. Ohne diese Techniken würden Feature-Dateien schnell redundant, lang und schwer wartbar werden.

### 4.1 Scenario Outline — Dosierungsvalidierung

```gherkin
Feature: Medikamentendosierung validieren

  Scenario Outline: Dosierungsgrenze für <medikament>
    Given patient "Anna Klein" weighs <gewicht> kg
    When the doctor prescribes <dosis> mg of "<medikament>"
    Then the system should show "<ergebnis>"
    And the alert level should be "<alert>"

    Examples:
      | medikament   | gewicht | dosis | ergebnis                        | alert   |
      | Paracetamol  | 70      | 500   | Dosierung akzeptiert            | NONE    |
      | Paracetamol  | 70      | 5000  | Tagesdosis überschritten        | HIGH    |
      | Ibuprofen    | 50      | 200   | Dosierung akzeptiert            | NONE    |
      | Ibuprofen    | 50      | 2400  | Maximaldosis für Gewicht prüfen | MEDIUM  |
      | Morphin      | 80      | 10    | Betäubungsmittel: Bestätigung   | INFO    |
      | Morphin      | 80      | 100   | Gefährliche Überdosierung       | CRITICAL|
```

### 4.2 Data Tables — Komplexe medizinische Daten

```gherkin
Feature: Laborwerte erfassen

  Scenario: Arzt erfasst Blutbild
    Given patient "Peter Wagner" had a blood test on "2024-03-15"
    When the following lab results are entered:
      | Parameter    | Wert  | Einheit | Referenzbereich | Status    |
      | Hämoglobin   | 11.2  | g/dL    | 13.5-17.5       | LOW       |
      | Leukozyten   | 12.8  | 10³/µL  | 4.5-11.0        | HIGH      |
      | Thrombozyten | 220   | 10³/µL  | 150-400         | NORMAL    |
      | Kreatinin    | 1.8   | mg/dL   | 0.7-1.2         | HIGH      |
    Then a clinical summary should be generated
    And 2 critical values should be flagged
    And the doctor should receive an alert for "Anämie-Verdacht"
```

**Data Table in C#:**

```csharp
public class Laborwert
{
    public string Parameter { get; set; } = "";
    public double Wert { get; set; }
    public string Einheit { get; set; } = "";
    public string Referenzbereich { get; set; } = "";
    public string Status { get; set; } = "";
}

[When(@"the following lab results are entered:")]
public void WennLaborwerteErfasst(DataTable tabelle)
{
    // Methode 1: Manuell
    foreach (var zeile in tabelle.Rows)
    {
        string parameter = zeile["Parameter"];
        double wert = double.Parse(zeile["Wert"], CultureInfo.InvariantCulture);
        string status = zeile["Status"];

        _db.Laborwerte.Add(new Laborwert
        {
            Parameter = parameter,
            Wert = wert,
            Status = status,
            PatientId = _context.Get<Patient>("AktuellerPatient").Id
        });
    }
    _db.SaveChanges();

    // Methode 2: Automatisch (empfohlen)
    var laborwerte = tabelle.CreateSet<Laborwert>().ToList();
    _context.Set(laborwerte, "Laborwerte");
}

[Then(@"(\d+) critical values should be flagged")]
public void DannSolltenKritischeWerteFlaggedSein(int erwartet)
{
    var laborwerte = _context.Get<List<Laborwert>>("Laborwerte");
    int kritisch = laborwerte.Count(l => l.Status is "HIGH" or "LOW" or "CRITICAL");
    kritisch.Should().Be(erwartet);
}
```

### 🏋️ Übung 4.1

Erstelle ein `Scenario Outline` für die Validierung von Vitalwerten:

| Vitalwert | Messwert | Einheit | Erwarteter Status |
|-----------|----------|---------|-------------------|
| Blutdruck | 120/80 | mmHg | NORMAL |
| Blutdruck | 180/110 | mmHg | KRITISCH — Hypertoniekrise |
| Puls | 72 | bpm | NORMAL |
| Puls | 145 | bpm | WARNUNG — Tachykardie |
| Sauerstoffsättigung | 98 | % | NORMAL |
| Sauerstoffsättigung | 88 | % | KRITISCH — Hypoxie |

### 4.3 Tageszusammenfassung

Heute hast du gelernt, wann du Scenario Outlines statt vieler einzelner Szenarien einsetzen solltest und wie Examples-Tabellen Wiederholungen vermeiden.

Zusätzlich hast du gesehen, wie Data Tables komplexe medizinische Eingabedaten übersichtlich machen und sich direkt in C#-Objekte oder Szenariokontext überführen lassen.

---

## 📘 TAG 5 — EF Core InMemory DB in Tests

### 5.0 Was du heute lernst und warum

Heute verschiebst du die Tests von reinem Logikcode auf eine realistischere Integrationsschicht. Mit **EF Core InMemory** kannst du Datenbankzugriffe und Persistenzverhalten in Tests simulieren, ohne einen echten SQL Server starten zu müssen.

Das ist besonders nützlich, wenn du prüfen willst, ob Daten korrekt gespeichert, aktualisiert und für Folgeprüfungen wieder gelesen werden. Gleichzeitig lernst du auch die Grenzen dieses Ansatzes kennen, damit du InMemory sinnvoll einsetzt und nicht mit einer echten Datenbank verwechselst.

### 5.1 Domain-Modelle (MedTech)

```csharp
// Models/Patient.cs
public class Patient
{
    public int Id { get; set; }
    public string PatientId { get; set; } = "";          // z.B. "P-4421"
    public string Name { get; set; } = "";
    public DateTime Geburtsdatum { get; set; }
    public string? BlutgruppeType { get; set; }
    public List<string> Allergien { get; set; } = new();
    public List<string> AktiveMedikamente { get; set; } = new();
    public List<Rezept> Rezepte { get; set; } = new();
    public List<Laborwert> Laborwerte { get; set; } = new();
}

// Models/Rezept.cs
public class Rezept
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public int ArztId { get; set; }
    public Arzt Arzt { get; set; } = null!;
    public string Medikament { get; set; } = "";
    public string Dosierung { get; set; } = "";
    public DateTime AusstellungsDatum { get; set; }
    public bool IstAktiv { get; set; }
    public string? Hinweis { get; set; }
}

// Models/Arzt.cs
public class Arzt
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Fachrichtung { get; set; } = "";
    public string Lanzummer { get; set; } = "";          // Arztnummer (MDR-relevant)
    public bool IstEingeloggt { get; set; }
}

// Models/AuditLogEintrag.cs (wichtig für MDR/FDA-Compliance)
public class AuditLogEintrag
{
    public int Id { get; set; }
    public string Aktion { get; set; } = "";
    public string Benutzer { get; set; } = "";
    public string EntityTyp { get; set; } = "";
    public int EntityId { get; set; }
    public string Änderungen { get; set; } = "";
    public DateTime Zeitstempel { get; set; }
}
```

### 5.2 TestDbContext mit InMemory

**Warum ein Interface?**

Das `IMedTechDbContext`-Interface wendet das **Dependency Inversion Principle** an: Der `RezeptService` hängt von einer Abstraktion ab, nicht von der konkreten EF-Core-Klasse. Das ermöglicht den Austausch der Implementierung je nach Kontext:

```
RezeptService → IMedTechDbContext ← TestDbContext     (InMemory, isoliert, für Tests)
                                  ← MedTechDbContext  (SQL Server, für Produktion)
```

Damit bleibt der `RezeptService` unverändert, egal ob im Test oder im echten System.

> **Wichtige Einschränkung:** EF Core InMemory verhält sich nicht vollständig wie eine echte SQL Server- oder PostgreSQL-Datenbank. Constraints, Transaktionen und spezifisches SQL-Verhalten werden ignoriert. Das ist für isolierte Tests auf der Business-Logik-Ebene vollkommen in Ordnung — aber pass auf:
>
> - Constraint-Verletzungen (z.B. `UNIQUE`) werden von InMemory **nicht** geworfen
> - Komplexe SQL-Abfragen (Raw SQL) funktionieren **nicht**
> - Für realistischere Integrationstests auf einem echten DB-Schema → später **Testcontainers** nutzen (SQL Server oder PostgreSQL in Docker)
>
> **Für diesen Kurs:** InMemory ist ideal. Für Production-grade Tests im echten Projekt: Testcontainers evaluieren.

```csharp
// Infrastructure/IMedTechDbContext.cs
// Interface trennt Business-Logik von der Test-Implementierung
public interface IMedTechDbContext
{
    DbSet<Patient> Patienten { get; }
    DbSet<Arzt> Aerzte { get; }
    DbSet<Rezept> Rezepte { get; }
    DbSet<Laborwert> Laborwerte { get; }
    DbSet<AuditLogEintrag> AuditLog { get; }
    int SaveChanges();
}

// Infrastructure/TestDbContext.cs
using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext, IMedTechDbContext
{
    public DbSet<Patient> Patienten => Set<Patient>();
    public DbSet<Arzt> Aerzte => Set<Arzt>();
    public DbSet<Rezept> Rezepte => Set<Rezept>();
    public DbSet<Laborwert> Laborwerte => Set<Laborwert>();
    public DbSet<AuditLogEintrag> AuditLog => Set<AuditLogEintrag>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // JSON-Konvertierung für Listen (EF Core 8)
        builder.Entity<Patient>()
            .Property(p => p.Allergien)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        builder.Entity<Patient>()
            .Property(p => p.AktiveMedikamente)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
    }

    // Testdaten für jeden Test-Lauf
    public void SeedTestData()
    {
        // Standard-Medikamentenliste (Lookup)
        // In echter App würde das aus einer echten DB kommen
        Database.EnsureCreated();
    }
}
```

### 5.3 InMemory DB in Steps verwenden

```csharp
// Infrastructure/RezeptService.cs — die echte Business-Logik
public class RezeptService
{
    private readonly IMedTechDbContext _db;

    // Bekannte gefährliche Kombinationen (vereinfacht)
    private readonly Dictionary<string, string[]> _interaktionen = new()
    {
        ["Warfarin"] = ["Aspirin", "Ibuprofen", "Naproxen"],
        ["Metformin"] = ["Alkohol", "Röntgenkontrastmittel"],
        ["MAO-Hemmer"] = ["SSRI", "Tramadol", "Triptane"]
    };

    // Bekannte Kreuzallergien
    private readonly Dictionary<string, string[]> _kreuzallergien = new()
    {
        ["Penicillin"] = ["Amoxicillin", "Ampicillin", "Piperacillin"],
        ["Sulfonamide"] = ["Trimethoprim-Sulfamethoxazol"]
    };

    public RezeptService(IMedTechDbContext db) => _db = db;

    public RezeptErgebnis VerschreibeMedikament(
        Patient patient, Arzt arzt, string medikament, string dosierung)
    {
        // 1. Allergie-Prüfung
        foreach (var allergen in patient.Allergien)
        {
            if (_kreuzallergien.TryGetValue(allergen, out var kreuzallergene))
            {
                if (kreuzallergene.Contains(medikament))
                {
                    return new RezeptErgebnis
                    {
                        ErfolgreichGespeichert = false,
                        Warnung = $"Patient is allergic to {allergen}-class antibiotics",
                        WarnungSchweregrad = "HIGH"
                    };
                }
            }
        }

        // 2. Wechselwirkungscheck
        foreach (var aktivMedikament in patient.AktiveMedikamente)
        {
            if (_interaktionen.TryGetValue(aktivMedikament, out var gefährlich))
            {
                if (gefährlich.Contains(medikament))
                {
                    return new RezeptErgebnis
                    {
                        ErfolgreichGespeichert = false,
                        Warnung = $"Increased bleeding risk with {aktivMedikament}",
                        WarnungSchweregrad = "HIGH",
                        Vorschlag = "consider alternative: Paracetamol"
                    };
                }
            }
        }

        // 3. Rezept speichern
        var rezept = new Rezept
        {
            Patient = patient,
            Arzt = arzt,
            Medikament = medikament,
            Dosierung = dosierung,
            AusstellungsDatum = DateTime.Now,
            IstAktiv = true
        };

        _db.Rezepte.Add(rezept);
        _db.SaveChanges(); // Erst speichern, damit rezept.Id generiert wird

        // 4. Audit-Log (MDR-Pflicht!)
        _db.AuditLog.Add(new AuditLogEintrag
        {
            Aktion = "REZEPT_ERSTELLT",
            Benutzer = arzt.Name,
            EntityTyp = "Rezept",
            EntityId = rezept.Id, // Jetzt korrekt gesetzt (nach erstem SaveChanges)
            Änderungen = $"Medikament: {medikament}, Dosierung: {dosierung}",
            Zeitstempel = DateTime.UtcNow
        });

        _db.SaveChanges();

        return new RezeptErgebnis
        {
            ErfolgreichGespeichert = true,
            PdfPfad = $"/rezepte/{rezept.Id}.pdf"
        };
    }
}

public class RezeptErgebnis
{
    public bool ErfolgreichGespeichert { get; set; }
    public string? Warnung { get; set; }
    public string? WarnungSchweregrad { get; set; }
    public string? Vorschlag { get; set; }
    public string? PdfPfad { get; set; }
}
```

### 🏋️ Übung 5.1 — InMemory DB Test

Schreibe einen vollständigen Test für:

```gherkin
Scenario: Audit log is created when prescription is issued
  Given Dr. Schmidt is logged in (license: "BAY-12345")
  And patient "Thomas Braun" (ID: P-7823) is open
  When Dr. Schmidt prescribes "Metformin 500mg" once daily
  Then the prescription should be saved
  And the audit log should contain an entry with:
    | Feld     | Wert              |
    | Aktion   | REZEPT_ERSTELLT   |
    | Benutzer | Dr. Schmidt       |
    | EntityTyp| Rezept            |
```

### 5.4 Tageszusammenfassung

Heute hast du verstanden, wie InMemory-Datenbanken Integrationstests vereinfachen und warum sie gut für isolierte, schnelle Tests geeignet sind.

Außerdem hast du gelernt, wo die Grenzen liegen: InMemory hilft dir bei Geschäftslogik und Persistenzfluss, ersetzt aber keine echte relationale Datenbank für Constraints, SQL-spezifisches Verhalten oder produktionsnahe Integrationsprüfungen.

---

## 📘 TAG 6 — Playwright + Blazor UI-Tests

### 6.0 Was du heute lernst und warum

Heute geht es um die oberste Spitze der Testpyramide: echte Browser- und UI-Tests. **Playwright** steuert den Browser, damit du prüfen kannst, was der Anwender wirklich sieht und tut. Im Blazor-Kontext bedeutet das zum Beispiel: Seiten aufrufen, Formulare befüllen, Warnungen sehen und UI-Zustände validieren.

Wichtig ist dabei nicht nur das Tool selbst, sondern auch die Abgrenzung: UI-Tests sind wertvoll, aber teuer. Du lernst heute deshalb auch, **wann** Playwright sinnvoll ist und wann ein Service- oder Integrationstest die bessere Wahl bleibt.

### ⚠️ Wichtig: BDD ≠ nur UI-Tests

Ein häufiger Fehler ist, BDD direkt mit UI-Tests gleichzusetzen. Das führt zu fragilen, wartungsintensiven Tests. Erinnere dich an die **Test-Pyramide** aus der Kurseinleitung: E2E-Tests sind ~10% — das meiste Testverhalten gehört in Service- oder Integrationstests.

Die konkrete Schichtung für diesen Kurs:

```
Feature (Gherkin — business-lesbar)
         ↓
Service-Layer Steps  ← das MEISTE steht hier
(Domain-Logik, InMemory DB, keine UI)
         ↓
UI Steps (optional, nur für echte E2E-Szenarien)
(Playwright, Blazor, echte Browser-Interaktion)
```

**Konkretes Beispiel:** Die Allergie-Warnung beim Verschreiben von Amoxicillin testen:

🟢 **Richtig (Service Layer):** `RezeptService.VerschreibeMedikament()` aufrufen → Ergebnis prüfen → fertig. Kein Browser, kein Warten, 10ms.

🔴 **Falsch (alles in UI):** Browser öffnen → einloggen → Patient suchen → Formular ausfüllen → Button klicken → Warnung prüfen → Browser schließen. 30 Sekunden. Bricht bei jedem UI-Redesign.

> **Faustregel:** Playwright einsetzen, wenn das Szenario **explizit das Verhalten der UI selbst** prüft — z.B. dass die Warnung rot und oben auf der Seite erscheint, oder dass ein bestimmter Button deaktiviert wird.

### 6.1 Vorbedingung: Es muss eine laufende Ziel-App geben

Playwright testet **keine Feature-Datei direkt** und auch **kein Page Object direkt**. Playwright steuert immer eine echte laufende Anwendung im Browser.

Für diesen Kurs ist die Ziel-App eine kleine Demo-Blazor-App mit stabilen `data-testid`-Attributen:

```
MedTech/
├── MedTech.Tests/   ← Reqnroll + Playwright Tests
└── MedTech.App/     ← laufende Blazor-Demo-App als Testziel
```

Die Kette sieht so aus:

```
BlazerUITests.feature
  ↓
PatientenakteUISteps.cs
  ↓
PatientenlistePage.cs / RezeptPage.cs
  ↓
laufende Blazor-App unter /patienten
```

Das bedeutet konkret:

- `BlazerUITests.feature` beschreibt das gewünschte Verhalten.
- `PatientenakteUISteps.cs` übersetzt die Gherkin-Schritte in Code.
- `PatientenlistePage.cs` kapselt Browser-Interaktionen.
- `MedTech.App` ist die reale Oberfläche, die im Browser geöffnet wird.

Wenn die Ziel-App nicht läuft, schlägt Playwright mit `ERR_CONNECTION_REFUSED` fehl. Das ist kein Reqnroll-Problem, sondern bedeutet schlicht: **Es gibt nichts, womit der Browser sprechen kann.**

### 6.2 Demo-App starten

```bash
# Terminal 1: Blazor-Demo-App starten
dotnet run --project MedTech.App/MedTech.App.csproj

# Optional: andere Basis-URL für die Tests setzen
set MEDTECH_UI_BASE_URL=http://localhost:3000

# Terminal 2: Browser-Tests ausführen
dotnet test MedTech.Tests/MedTech.Tests.csproj --filter "Category=browser"
```

Im Browser kannst du die Zielseite auch manuell öffnen:

```text
http://localhost:3000/patienten
```

### 6.3 Playwright Setup

```bash
# Nach dem Build: Playwright-Browser herunterladen
dotnet build MedTech.Tests/MedTech.Tests.csproj
powershell -File MedTech.Tests/bin/Debug/net8.0/playwright.ps1 install chromium
```

### 6.4 Page Object Model für Blazor

```csharp
// Pages/BasePage.cs
using Microsoft.Playwright;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected async Task<ILocator> WarteFürElement(string selector)
    {
        var element = Page.Locator(selector);
        await element.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        return element;
    }

    protected async Task KlickeAuf(string selector)
    {
        await (await WarteFürElement(selector)).ClickAsync();
    }

    protected async Task GibeEin(string selector, string text)
    {
        var element = await WarteFürElement(selector);
        await element.ClearAsync();
        await element.FillAsync(text);
    }

    protected async Task<string> LeseText(string selector)
    {
        return await Page.Locator(selector).InnerTextAsync();
    }

    // Blazor-spezifisch: Warte auf Blazor-Rendering
    protected async Task WarteBlazerRendering()
    {
        // Warte bis Blazor fertig gerendert hat
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
```

```csharp
// Pages/PatientenlistePage.cs
using Microsoft.Playwright;

public class PatientenlistePage : BasePage
{
    // Selektoren für Blazor-Komponenten
    // Tipp: Nutze data-testid Attribute in Blazor-Komponenten!
    private const string SuchfeldSelector = "[data-testid='patient-search-input']";
    private const string SuchergebnisSelector = "[data-testid='patient-result-item']";
    private const string PatientenakteSelector = "[data-testid='patient-record']";
    private const string FehlerMeldungSelector = "[data-testid='error-message']";
    private const string LadeindikatorSelector = ".loading-spinner";

    private readonly string _basisUrl;

    public PatientenlistePage(IPage page, string basisUrl) : base(page)
    {
        _basisUrl = basisUrl;
    }

    public async Task NavigiereZurPatientenliste()
    {
        await Page.GotoAsync($"{_basisUrl}/patienten");
        await WarteBlazerRendering();
    }

    public async Task SucheNachPatient(string suchbegriff)
    {
        await GibeEin(SuchfeldSelector, suchbegriff);
        // Blazor debounced Search — kurz warten
        await Page.WaitForTimeoutAsync(500);
        await WarteBlazerRendering();
    }

    public async Task<int> AnzahlSuchergebnisse()
    {
        return await Page.Locator(SuchergebnisSelector).CountAsync();
    }

    public async Task<string> ErsterSuchertreffer()
    {
        return await Page.Locator(SuchergebnisSelector).First.InnerTextAsync();
    }

    public async Task ÖffnePatientenakte(string patientName)
    {
        var link = Page.Locator(SuchergebnisSelector)
                       .Filter(new() { HasText = patientName });
        await link.ClickAsync();
        await WarteBlazerRendering();
    }

    public async Task<bool> IstPatientenakteGeöffnet()
    {
        return await Page.Locator(PatientenakteSelector).IsVisibleAsync();
    }

    public async Task<bool> IstFehlermeldungSichtbar()
    {
        return await Page.Locator(FehlerMeldungSelector).IsVisibleAsync();
    }
}
```

```csharp
// Pages/RezeptPage.cs
using Microsoft.Playwright;

public class RezeptPage : BasePage
{
    private const string MedikamentInput = "[data-testid='medication-input']";
    private const string DosierungInput = "[data-testid='dosage-input']";
    private const string VerschreibenButton = "[data-testid='prescribe-button']";
    private const string AllergieWarnung = "[data-testid='allergy-warning']";
    private const string WechselwirkungWarnung = "[data-testid='interaction-warning']";
    private const string ErfolgsMeldung = "[data-testid='success-message']";
    private const string WarnungSchweregrad = "[data-testid='warning-severity']";

    public RezeptPage(IPage page) : base(page) { }

    public async Task GibeMedikamentEin(string medikament)
    {
        await GibeEin(MedikamentInput, medikament);
        // Warte auf Autocomplete/Validierung
        await WarteBlazerRendering();
    }

    public async Task GibeDosierungEin(string dosierung)
        => await GibeEin(DosierungInput, dosierung);

    public async Task KlickeVerschreiben()
    {
        await KlickeAuf(VerschreibenButton);
        await WarteBlazerRendering();
    }

    public async Task<bool> IstAllergieWarnungSichtbar()
        => await Page.Locator(AllergieWarnung).IsVisibleAsync();

    public async Task<string> LeseAllergieWarnungsText()
        => await LeseText(AllergieWarnung);

    public async Task<string> LeseWarnungSchweregrad()
        => await LeseText(WarnungSchweregrad);

    public async Task<bool> IstErfolgsMeldungSichtbar()
        => await Page.Locator(ErfolgsMeldung).IsVisibleAsync();

    // Screenshot bei Fehler
    public async Task<byte[]> MacheScreenshot()
        => await Page.ScreenshotAsync(new() { FullPage = true });
}
```

### 6.5 Playwright-Hooks für Blazor

```csharp
// Hooks/PlaywrightHooks.cs
using Microsoft.Playwright;
using Reqnroll;
using BoDi;

[Binding]
public class PlaywrightHooks
{
    private readonly IObjectContainer _container;
    private readonly ScenarioContext _context;

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPage? _page;

    private static readonly string AppUrl =
      Environment.GetEnvironmentVariable("MEDTECH_UI_BASE_URL")?.TrimEnd('/')
      ?? "http://localhost:3000";

    public PlaywrightHooks(IObjectContainer container, ScenarioContext context)
    {
        _container = container;
        _context = context;
    }

    [BeforeScenario("browser", Order = 20)]
    public async Task BrowserStarten()
    {
        _playwright = await Playwright.CreateAsync();

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Environment.GetEnvironmentVariable("CI") == "true",
            SlowMo = 100, // Etwas langsamer für bessere Debugging-Möglichkeiten
        });

        _browserContext = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new() { Width = 1920, Height = 1080 },
            Locale = "de-DE",
            // Für Auth (wenn nötig):
            // StorageStatePath = "auth-state.json"
        });

        _page = await _browserContext.NewPageAsync();

        // Seiten-Objekte erstellen und im Container registrieren
        _container.RegisterInstanceAs(_page);
        _container.RegisterInstanceAs(new PatientenlistePage(_page, AppUrl));
        _container.RegisterInstanceAs(new RezeptPage(_page));
    }

    [AfterScenario("browser")]
    public async Task NachBrowserSzenario()
    {
        // Bei Fehler: Screenshot speichern
        if (_context.TestError != null && _page != null)
        {
            var screenshot = await _page.ScreenshotAsync(new() { FullPage = true });
            var pfad = $"TestResults/screenshots/{_context.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            Directory.CreateDirectory("TestResults/screenshots");
            await File.WriteAllBytesAsync(pfad, screenshot);
            Console.WriteLine($"📸 Screenshot: {pfad}");
        }

        if (_browserContext != null) await _browserContext.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}
```

### 6.6 Vollständige UI-Steps für Blazor

```csharp
// StepDefinitions/PatientenakteUISteps.cs
[Binding]
public class PatientenakteUISteps
{
    private readonly PatientenlistePage _patientenlistePage;
    private readonly RezeptPage _rezeptPage;
    private readonly ScenarioContext _context;

    public PatientenakteUISteps(
        PatientenlistePage patientenlistePage,
        RezeptPage rezeptPage,
        ScenarioContext context)
    {
        _patientenlistePage = patientenlistePage;
        _rezeptPage = rezeptPage;
        _context = context;
    }

    [Given(@"Dr\. Müller is logged into the Blazor application")]
    public async Task GegebenDrMüllerIstEingeloggt()
    {
        await _patientenlistePage.NavigiereZurPatientenliste();
        // Login-Prozess (falls nötig)
    }

    [When(@"she searches for patient ""(.*)""")]
    public async Task WennSieNachPatientSucht(string name)
    {
        await _patientenlistePage.SucheNachPatient(name);
    }

    [Then(@"the patient record should be displayed")]
    public async Task DannSolltePatientenakteAngezeigtWerden()
    {
        (await _patientenlistePage.AnzahlSuchergebnisse())
            .Should().BeGreaterThan(0, "Es sollte mindestens ein Suchergebnis geben");

        await _patientenlistePage.ÖffnePatientenakte(_context.Get<string>("GesuchterPatient"));

        (await _patientenlistePage.IstPatientenakteGeöffnet())
            .Should().BeTrue("Die Patientenakte sollte geöffnet sein");
    }

    [When(@"the doctor prescribes ""(.*)"" in the UI")]
    public async Task WennArztVerschreibtInUI(string medikament)
    {
        await _rezeptPage.GibeMedikamentEin(medikament);
        await _rezeptPage.GibeDosierungEin("2x täglich");
        await _rezeptPage.KlickeVerschreiben();
    }

    [Then(@"a red allergy warning should appear in the UI")]
    public async Task DannSollteAllergieWarnungInUIErscheinen()
    {
        (await _rezeptPage.IstAllergieWarnungSichtbar())
            .Should().BeTrue("Die Allergie-Warnung muss für den Arzt sichtbar sein");
    }
}
```

### 6.7 Tageszusammenfassung

Heute hast du gelernt, wie UI-Tests mit Playwright aufgebaut werden und warum Page Objects helfen, Browserlogik sauber von fachlichen Schritten zu trennen.

Mindestens genauso wichtig: Du hast die Rolle von UI-Tests innerhalb der Testpyramide eingeordnet. Nicht alles gehört in den Browser, sondern nur das Verhalten, das wirklich in der Oberfläche verifiziert werden muss.

---

## 📘 TAG 7 — Mini-Projekt 1: Vollständige Patientenakte

### 7.0 Was du heute lernst und warum

Heute setzt du mehrere Kursbausteine gemeinsam ein. Das Mini-Projekt ist bewusst praxisnah aufgebaut: Du kombinierst Feature-Dateien, Step Definitions, Hooks, Datenhaltung und optional UI-Schritte zu einem zusammenhängenden Testpaket rund um die Patientenakte.

Der Zweck dieses Tages ist Transfer. Du sollst nicht nur einzelne Techniken kennen, sondern erleben, wie sie zusammenwirken, wenn ein echtes fachliches Feature in Tests übersetzt wird.

### Aufgabe

Implementiere alle Tests für die Patientenakten-Verwaltung:

```gherkin
@browser @regression @kritisch
Feature: Patientenakte verwalten
  As a treating physician
  I want to manage patient records
  So that I can provide safe and informed medical care

  Background:
    Given Dr. Hoffmann is logged into the MedTech system

  @smoke
  Scenario: Open patient record successfully
    Given patient "Eva Braun" (DOB: 1985-06-15) exists in the system
    When the doctor searches for "Eva Braun"
    And opens her patient record
    Then the following information should be visible:
      | Feld              | Wert              |
      | Name              | Eva Braun         |
      | Geburtsdatum      | 15.06.1985        |
      | Blutgruppe        | A+                |
      | Allergien         | Penicillin        |

  @sicherheit
  Scenario: Allergy banner is always visible on patient record
    Given patient "Max Roth" has allergies: "Penicillin, Latex, Aspirin"
    When the doctor opens Max Roth's record
    Then a permanent allergy banner should be shown at the top
    And the banner should list all 3 allergies
    And the banner should be in red/warning color

  @audit @regulatorisch
  Scenario: Record access is logged for compliance
    Given patient "Lisa Grün" exists in the system
    When the doctor opens her record
    Then an audit entry should be created with:
      | Aktion   | AKTE_GEÖFFNET    |
      | Benutzer | Dr. Hoffmann     |
    And the timestamp should be within the last 10 seconds

  @negativ
  Scenario: Patient not found shows helpful message
    When the doctor searches for "Nicht Existent"
    Then the message "Kein Patient gefunden" should appear
    And a suggestion to create a new patient should be shown
```

**Tipp: Projektstruktur für Mini-Projekt 1**

```
MedTech.Tests/
├── Features/Patientenakte.feature      ← Feature-File (oben)
├── StepDefinitions/
│   ├── PatientenakteApiSteps.cs        ← Business-Logik Steps (DB)
│   └── PatientenakteUISteps.cs         ← Blazor UI Steps (Playwright)
├── Pages/
│   └── PatientenlistePage.cs           ← Page Object
└── Hooks/
    ├── DatabaseHooks.cs                ← InMemory DB Setup
    └── PlaywrightHooks.cs              ← Browser Setup
```

### 7.1 Tageszusammenfassung

Heute hast du ein vollständigeres Fachfeature in Teilbereiche zerlegt und gesehen, wie Unit-, Integrations- und UI-Anteile sinnvoll zusammenkommen.

Der zentrale Lerneffekt dieses Tages ist Architekturverständnis: Gute Testlösungen entstehen nicht aus einem einzelnen Testtyp, sondern aus sauber kombinierten Schichten mit klarer Verantwortung.

---

## 📘 TAG 8 — API-Testing

### 8.0 Was du heute lernst und warum

Heute prüfst du das Verhalten des Systems über seine HTTP-Schnittstellen. API-Tests sind wichtig, weil sie Geschäftsverhalten und Systemintegration abdecken, ohne die zusätzliche Fragilität von Browser-Tests mitzubringen.

Du lernst an diesem Tag, wie Authentifizierung, Statuscodes, Response-Inhalte und Sicherheitsanforderungen testbar gemacht werden. Gerade in MedTech ist das relevant, weil externe Systeme, Rollenmodelle und Datenschutzregeln oft über APIs abgesichert werden.

### 8.1 MedTech REST API Feature

```gherkin
@api @regression
Feature: Patientendaten-API
  As a system integrator
  I want to ensure the patient data API works correctly
  So that external systems (hospital ERP, pharmacy) can integrate reliably

  Background:
    Given the API base URL is "https://api.medtech.de/v1"
    And I am authenticated as API user with role "PHYSICIAN"

  @smoke @get
  Scenario: Retrieve patient data
    Given patient "P-4421" exists in the system
    When I request the patient record for "P-4421"
    Then the response status should be 200
    And the response should contain:
      | Feld      | Wert        |
      | patientId | P-4421      |
      | name      | Hans Schmidt|
    And sensitive internal data should not be exposed in the response

  @post
  Scenario: Create new patient record via API
    When I send POST "/patients" with body:
      """json
      {
        "firstName": "Anna",
        "lastName": "Wagner",
        "dateOfBirth": "1990-03-22",
        "bloodType": "B+",
        "allergies": ["Aspirin"]
      }
      """
    Then the response status should be 201
    And the response should contain field "patientId"
    And the new patient should exist in the database

  @sicherheit @negativ
  Scenario: Unauthorized access returns 401
    Given I am NOT authenticated
    When I send GET "/patients/P-4421"
    Then the response status should be 401
    And the response should contain "Unauthorized"

  @datenschutz @negativ
  Scenario: Physician cannot access patient from different practice
    Given patient "P-9999" belongs to practice "Praxis Müller"
    And I am authenticated as physician from practice "Praxis Schmidt"
    When I send GET "/patients/P-9999"
    Then the response status should be 403
    And the response should contain "Access denied"
```

### 8.2 API Steps in C#

```csharp
// StepDefinitions/ApiSteps.cs
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reqnroll;
using FluentAssertions;

[Binding]
public class ApiSteps
{
    private readonly ScenarioContext _context;
    private readonly TestDbContext _db;
    private RestClient _client = null!;
    private RestResponse _antwort = null!;

    public ApiSteps(ScenarioContext context, TestDbContext db)
    {
        _context = context;
        _db = db;
    }

    [Given(@"the API base URL is ""(.*)""")]
    public void GegebenApiBasisUrl(string url)
    {
        _client = new RestClient(url);
    }

    [Given(@"I am authenticated as API user with role ""(.*)""")]
    public async Task GegebenIchBinAuthentifiziert(string rolle)
    {
        var loginReq = new RestRequest("/auth/login", Method.Post);
        loginReq.AddJsonBody(new
        {
            username = "test_arzt@medtech.de",
            password = "TestPass123!",
            role = rolle
        });

        var loginResp = await _client.ExecuteAsync(loginReq);
        var token = JObject.Parse(loginResp.Content!)["token"]?.ToString();

        token.Should().NotBeNullOrEmpty("Login muss JWT-Token zurückgeben");
        _client.AddDefaultHeader("Authorization", $"Bearer {token}");
    }

    [Given(@"I am NOT authenticated")]
    public void GegebenNichtAuthentifiziert()
    {
        // Kein Auth-Header → 401 erwartet
        _client.RemoveDefaultParameter("Authorization");
    }

    [When(@"I send GET ""(.*)""")]
    public async Task WennGetAnfrage(string pfad)
    {
        var req = new RestRequest(pfad, Method.Get);
        _antwort = await _client.ExecuteAsync(req);
    }

    [When(@"I send POST ""(.*)"" with body:")]
    public async Task WennPostMitBody(string pfad, string body)
    {
        var req = new RestRequest(pfad, Method.Post);
        req.AddStringBody(body, ContentType.Json);
        _antwort = await _client.ExecuteAsync(req);
    }

    [Then(@"the response status should be (\d+)")]
    public void DannStatusCode(int erwartet)
    {
        ((int)_antwort.StatusCode).Should().Be(erwartet,
            $"Antwort-Body: {_antwort.Content}");
    }

    [Then(@"the response should contain:")]
    public void DannAntwortSollEnthalten(DataTable tabelle)
    {
        var json = JObject.Parse(_antwort.Content!);
        foreach (var zeile in tabelle.Rows)
        {
            string feld = zeile["Feld"];
            string erwartet = zeile["Wert"];
            json[feld]?.ToString().Should().Be(erwartet,
                $"Feld '{feld}' hat falschen Wert");
        }
    }

    [When(@"I request the patient record for ""(.*)""")]
    public async Task WennPatientenakteAngefordert(string patientId)
    {
        var req = new RestRequest($"/patients/{patientId}", Method.Get);
        _antwort = await _client.ExecuteAsync(req);
    }

    [Then(@"sensitive internal data should not be exposed in the response")]
    public void DannSensibleDatenNichtSichtbar()
    {
        // Technische Prüfung HINTER einem fachlich lesbaren Step versteckt
        var json = JObject.Parse(_antwort.Content!);
        json["passwordHash"].Should().BeNull("Passwort-Hashes dürfen nie in API-Antworten erscheinen");
        json["internalNotes"].Should().BeNull("Interne Notizen sind nicht für externe Clients bestimmt");
    }

    [Then(@"the new patient should exist in the database")]
    public void DannPatientSollInDbExistieren()
    {
        var json = JObject.Parse(_antwort.Content!);
        string? patientId = json["patientId"]?.ToString();

        patientId.Should().NotBeNullOrEmpty();

        var patient = _db.Patienten.FirstOrDefault(p => p.PatientId == patientId);
        patient.Should().NotBeNull($"Patient {patientId} muss in der DB gespeichert sein");
    }
}
```

### 8.3 Lokale API-Tests ohne externen Server (WebApplicationFactory)

Das obige Beispiel sendet HTTP-Anfragen an `https://api.medtech.de/v1` — das funktioniert nur, wenn dieser Server läuft. Für vollständig isolierte Tests ohne Netzwerkabhängigkeit bietet `Microsoft.AspNetCore.Mvc.Testing` die `WebApplicationFactory`:

```csharp
// Startet die Blazor-/API-App im selben Testprozess — kein externer Server nötig
using Microsoft.AspNetCore.Mvc.Testing;

[Binding]
public class LocalApiSteps
{
    private readonly HttpClient _client;

    public LocalApiSteps(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Echte DB-Registrierung entfernen, InMemory einhängen:
                services.AddDbContext<MedTechDbContext>(opt =>
                    opt.UseInMemoryDatabase($"ApiTest_{Guid.NewGuid()}"));
            });
        }).CreateClient();
    }

    [When(@"I send GET ""(.*)""")]
    public async Task WennGetAnfrage(string pfad)
    {
        _antwort = await _client.GetAsync(pfad);
    }
}
```

**Vergleich der beiden Ansätze:**

| | RestSharp (externe URL) | WebApplicationFactory |
|--|------------------------|-----------------------|
| Server nötig? | ✅ Ja | ❌ Nein |
| Lernaufwand | Niedrig | Mittel |
| Isolation | Gering | Hoch |
| Geeignet für | Manuelle API-Tests | CI/CD, vollautomatisch |

> **Für diesen Kurs:** RestSharp mit externer URL ist einfacher zu verstehen. `WebApplicationFactory` ist die Production-grade Alternative — zeig im Interview, dass du sie kennst.

### 8.4 Tageszusammenfassung

Heute hast du gelernt, wie API-Tests Fachverhalten, Sicherheitsregeln und Datenflüsse auf einer stabileren Schicht als im UI prüfen können.

Du hast außerdem gesehen, dass gute API-Tests nicht nur Statuscodes kontrollieren, sondern auch Rollen, Datenschutz, Response-Inhalte und Seiteneffekte in der Datenbank verifizieren.

---

## 📘 TAG 9 — Reporting & CI/CD

### 9.0 Was du heute lernst und warum

Heute geht es nicht mehr nur um einzelne Tests, sondern um den Betrieb einer Test-Suite im Team. **Reporting** macht Testergebnisse nachvollziehbar, auffindbar und auditierbar. **CI/CD** sorgt dafür, dass Tests automatisiert bei Änderungen, Pull Requests oder nach Zeitplan laufen.

Das ist in MedTech besonders wichtig, weil Testnachweise häufig längerfristig verfügbar sein müssen und unterschiedliche Testarten getrennt steuerbar sein sollten, etwa schnelle Smoke-Tests, Browser-Tests und regulatorische Prüfungen.

### 9.1 Allure Reports (empfohlen für MedTech)

```xml
<PackageReference Include="Allure.Reqnroll" Version="2.12.1" />
```

```json
// allureConfig.json
{
  "allure": {
    "directory": "allure-results",
    "title": "MedTech BDD Test Report",
    "links": [
      {
        "type": "issue",
        "urlTemplate": "https://jira.medtech.de/browse/{}"
      },
      {
        "type": "tms",
        "urlTemplate": "https://testrail.medtech.de/testcases/{}"
      }
    ]
  }
}
```

```bash
# Report generieren
allure generate allure-results --clean -o allure-report
allure open allure-report
```

### 9.2 GitHub Actions für MedTech

```yaml
# .github/workflows/medtech-tests.yml
name: MedTech BDD Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  schedule:
    # Täglich um 6 Uhr Regression-Tests
    - cron: '0 6 * * *'

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  unit-and-integration-tests:
    name: Unit & Integration Tests (InMemory DB)
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET ${{ env.DOTNET_VERSION }}
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Run non-browser tests
      run: |
        dotnet test --no-build \
          --configuration Release \
          --filter "Category!=browser" \
          --logger "trx;LogFileName=unit-results.trx" \
          --results-directory ./TestResults

    - name: Upload test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: unit-test-results
        path: TestResults/*.trx

  e2e-browser-tests:
    name: E2E Browser Tests (Playwright + Blazor)
    runs-on: ubuntu-latest
    needs: unit-and-integration-tests

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: Build
      run: dotnet build --configuration Release

    - name: Install Playwright browsers
      run: pwsh bin/Release/net8.0/playwright.ps1 install --with-deps chromium

    - name: Start Blazor test server
      run: |
        dotnet run --project src/MedTech.BlazorApp \
          --environment Testing &
        sleep 10  # Warte bis Server gestartet

    - name: Run browser tests
      env:
        CI: "true"
        APP_URL: "https://localhost:5001"
      run: |
        dotnet test --no-build \
          --configuration Release \
          --filter "Category=browser" \
          --logger "trx;LogFileName=e2e-results.trx" \
          --results-directory ./TestResults

    - name: Upload screenshots on failure
      uses: actions/upload-artifact@v4
      if: failure()
      with:
        name: playwright-screenshots
        path: TestResults/screenshots/

  regulatory-compliance-tests:
    name: Regulatory Tests (MDR/Audit)
    runs-on: ubuntu-latest
    needs: unit-and-integration-tests

    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: Run regulatory tests
      run: |
        dotnet test --filter "Category=regulatorisch" \
          --logger "trx;LogFileName=regulatory-results.trx" \
          --results-directory ./TestResults

    - name: Archive compliance report
      uses: actions/upload-artifact@v4
      with:
        name: compliance-report
        path: TestResults/regulatory-results.trx
        retention-days: 365  # MDR: Aufbewahrungspflicht
```

### 9.3 Tageszusammenfassung

Heute hast du verstanden, warum Testautomatisierung erst dann teamtauglich wird, wenn Ergebnisse sichtbar, archiviert und reproduzierbar ausgeführt werden können.

Du hast außerdem gelernt, wie CI/CD Testarten trennt, Reports erzeugt und regulatorisch wichtige Ergebnisse langfristig bereitstellen kann.

---

## 📘 TAG 10 — Mini-Projekt 2: E2E Rezept-Workflow

### 10.0 Was du heute lernst und warum

Heute verbindest du mehrere Ebenen zu einem echten End-to-End-Ablauf: UI, API und Audit-Log. Der Fokus liegt darauf zu verstehen, wie ein fachlich kritischer Prozess vom Einstieg im Frontend bis zur technischen Nachverifikation durchgängig getestet werden kann.

Dieser Tag ist bewusst anspruchsvoll. Er zeigt dir, wie teuer, aber auch wie aussagekräftig E2E-Tests sein können, wenn sie gezielt für besonders wichtige Workflows eingesetzt werden.

### Vollständiger End-to-End-Test: Arzt verschreibt Medikament

```gherkin
@e2e @browser @api @kritisch
Feature: Vollständiger Rezept-Workflow (E2E)
  As a treating physician
  I want to safely prescribe medication through the complete workflow
  So that patients receive correct treatment and all regulations are met

  Background:
    Given the MedTech system is running
    And Dr. Sommer is logged in (license: "BAY-98765")

  @smoke @regulatorisch
  Scenario: Complete safe prescription workflow
    # Schritt 1: Patient suchen (UI)
    Given patient "Georg Fischer" (ID: P-5512) is in the system
      And he has no known allergies
      And he is currently taking no other medication

    # Schritt 2: Arzt öffnet Akte (UI)
    When Dr. Sommer opens Georg Fischer's patient record in the Blazor app
    Then the patient record should display correctly
    And no allergy banner should be shown

    # Schritt 3: Medikament verschreiben (UI)
    When the doctor prescribes "Amoxicillin 500mg" three times daily
    Then no warnings should appear
    And the prescription should be confirmed as saved
    And a prescription number should be generated

    # Schritt 4: Überprüfung via API
    When I query the API for patient P-5512's prescriptions
    Then the API should return 1 active prescription
    And the prescription should contain "Amoxicillin 500mg"

    # Schritt 5: Audit-Log (Regulatorisch)
    And the audit log should contain:
      | Aktion          | REZEPT_ERSTELLT |
      | Arzt-Lizenz     | BAY-98765       |
      | PatientenID     | P-5512          |
    And the audit timestamp should be UTC

  @sicherheit @e2e
  Scenario: Dangerous prescription is blocked end-to-end
    Given patient "Rita Becker" (ID: P-6634) is in the system
      And she is allergic to "Penicillin"
      And she is currently taking "Warfarin"

    When Dr. Sommer opens Rita Becker's record
    Then a red ALLERGY banner should be at the top of the page

    When the doctor tries to prescribe "Amoxicillin" (Penicillin-class)
    Then a critical allergy warning should block the prescription
    And the prescription should NOT be saved via API
    And the blocked attempt should be logged in the audit trail
```

### 10.1 Tageszusammenfassung

Heute hast du gesehen, wie ein geschäftskritischer Workflow durch mehrere Systemgrenzen hinweg getestet wird: von der UI über die API bis zur Auditierung.

Damit ist auch klar geworden, warum solche Tests sparsam eingesetzt werden sollten: Sie liefern starke Absicherung für zentrale Abläufe, sind aber deutlich aufwendiger als Unit- oder Integrationstests.

---

## 📘 TAG 11 — Best Practices & Profitipps

### 11.0 Was du heute lernst und warum

Heute geht es um Einordnung und Qualitätssicherung deiner bisherigen Arbeit. Best Practices helfen dir dabei, nicht nur funktionierende Tests zu schreiben, sondern Tests, die auch nach Wochen und Monaten noch verständlich, robust und wartbar bleiben.

Der Tag bündelt typische Fehlerbilder, sinnvolle Architekturentscheidungen und pragmatische Regeln für den Alltag. Damit bekommst du einen Rahmen, wie du künftige Tests schneller bewerten und verbessern kannst.

### 11.1 ✅ Do's — Gutes BDD in MedTech

```gherkin
# ✅ Beschreibt VERHALTEN aus Arzt-Perspektive
Scenario: Doctor is warned about critical drug interaction
  Given patient is taking Warfarin
  When doctor prescribes Aspirin
  Then a high-severity interaction warning should appear

# ✅ Wiederverwendbare, abstrakte Steps
Given Dr. Müller is logged in
# (Details wie Session-Management sind im Hook versteckt)

# ✅ Fachliche Begriffe, keine technischen
When the doctor opens the patient's allergy overview
# NICHT: When I click on button with id="allergy-tab-btn"

# ✅ Regulatorische Anforderungen explizit benennen
@mdr @audit-pflicht
Scenario: Prescription change is traceable in audit log
```

### 11.2 ❌ Don'ts — Was du vermeiden solltest

```gherkin
# ❌ UI-Implementierungsdetails (bricht bei jedem Redesign)
When I click on the blue button at position (350, 120)
When I find element with XPath "//div[@class='btn-primary'][3]"

# ❌ Zu viele Schritte (Antipattern "UI Scripting")
Scenario: Zu langer Test
  Given I open Chrome browser
  And I navigate to "https://app.de/login"
  And I wait 2 seconds
  And I click on the login field
  And I type "user@test.de" character by character
  ... (30 weitere Schritte)

# ❌ Technischer Jargon für medizinische Domäne
When the POST request to /api/v1/patients returns 201
# ✅ BESSER:
When the new patient record is successfully created
```

### 11.3 Dependency Injection — Reqnroll-Stil

```csharp
// Eigene Kontext-Klassen statt ScenarioContext für komplexe Szenarien

// PatientenTestContext.cs
public class PatientenTestContext
{
    public Patient? AktuellerPatient { get; set; }
    public Arzt? AktuellerArzt { get; set; }
    public RezeptErgebnis? LetzteRezeptAktion { get; set; }
    public List<string> GesammelteWarnungen { get; set; } = new();
}

// Wird automatisch von Reqnroll injiziert — selbe Instanz überall
[Binding]
public class PatientenSteps
{
    private readonly PatientenTestContext _patientenContext;
    private readonly TestDbContext _db;

    public PatientenSteps(PatientenTestContext ctx, TestDbContext db)
    {
        _patientenContext = ctx;
        _db = db;
    }
}

[Binding]
public class RezeptSteps
{
    private readonly PatientenTestContext _patientenContext; // Gleiche Instanz!

    public RezeptSteps(PatientenTestContext ctx)
    {
        _patientenContext = ctx;
    }

    [Then("the prescription should be created for the current patient")]
    public void DannRezeptFürAktuellenPatienten()
    {
        // Direkter Zugriff — kein ScenarioContext["key"] nötig
        _patientenContext.LetzteRezeptAktion!.ErfolgreichGespeichert
            .Should().BeTrue();
    }
}
```

### 11.4 InMemory DB — Häufige Fallstricke

```csharp
// ❌ FALSCH: Gleiche DB für alle Tests (Tests beeinflussen sich)
private static readonly DbContextOptions<TestDbContext> _options =
    new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase("SHARED_DB") // ← Bug! State bleibt zwischen Tests
        .Options;

// ✅ RICHTIG: Neue DB pro Szenario
[BeforeScenario]
public void NeueDatenbank()
{
    var options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase($"Test_{Guid.NewGuid()}") // ← Eindeutig!
        .Options;
    _container.RegisterInstanceAs(new TestDbContext(options));
}
```

### 11.5 Test-Pyramide im Alltag — Priorisierung

Ein häufiger Fehler in neuen Teams: **alles in E2E-Tests stecken**, weil es „vollständiger" wirkt. Die Konsequenz: langsame, fragile CI-Pipelines, die niemand mehr repariert.

```
Faustregel für jedes neue Feature in MedTech:

1. RezeptService.cs (Business Logic) → Unit-Tests schreiben  🟢
   → Allergie-Check, Wechselwirkung, Dosierung, Audit-Log

2. RezeptController.cs (API-Endpunkt) → Integrationstest   🟡
   → Wird 201 zurückgegeben? Ist der Audit-Eintrag in der DB?

3. RezeptPage.razor (UI) → genau 1–2 E2E-Tests             🔴
   → Nur: Arzt sieht Warnung rot auf der Seite (visuell, nicht logisch)
```

### 11.6 Testcontainers — der nächste Schritt nach InMemory

Wenn du InMemory gut verstehst, ist das der logische nächste Schritt für realistischere Integrationstests:

```csharp
// Nuget: Testcontainers.MsSql
await using var mssql = new MsSqlBuilder().Build();
await mssql.StartAsync();

var options = new DbContextOptionsBuilder<MedTechDbContext>()
    .UseSqlServer(mssql.GetConnectionString())
    .Options;

// Jetzt verhält sich die DB wie echtes SQL Server
// → Constraints, Transaktionen, Raw SQL funktionieren alle korrekt
```

> Testcontainers ist kein Pflichtthema für die ersten 1,5 Wochen — aber zeig, dass du es kennst. Das macht im Interview Eindruck.

### 11.7 Playwright — Blazor-spezifische Tipps

```csharp
// ✅ Immer auf Blazor-Rendering warten
await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

// ✅ data-testid statt CSS/XPath in Blazor-Komponenten verwenden
// In Blazor-Komponente:
// <button data-testid="prescribe-button" @onclick="VerschreibeMedikament">
// Im Test:
await page.Locator("[data-testid='prescribe-button']").ClickAsync();

// ✅ Auf spezifische Netzwerkanfragen warten (Blazor SignalR)
await page.WaitForResponseAsync("**/api/prescriptions");

// ✅ Retry-Logik für flaky Blazor-Animationen
await Assertions.Expect(page.Locator("[data-testid='success-toast']"))
    .ToBeVisibleAsync(new() { Timeout = 10000 });
```

### 11.8 Checkliste für deinen ersten Arbeitstag

```
GRUNDLAGEN (Woche 1 — muss sitzen):
□ Reqnroll als Community-Fork von SpecFlow erklären können
□ Feature-Files lesbar für Ärzte/QM-Manager (kein technischer Jargon)
□ Step Definitions: Cucumber Expressions und Regex kennen
□ InMemory DB: jedes Szenario hat eigene, isolierte DB (Guid als DB-Name)
□ InMemory DB Einschränkungen kennen (keine Constraints, kein Raw SQL)
□ Hooks: DB-Init, Setup/Teardown, Screenshot bei Fehlern
□ Tags konsequent: @smoke, @sicherheit, @regulatorisch, @api, @browser
□ Test-Pyramide erklären können: 70% Unit / 20% Integration / 10% E2E

FORTGESCHRITTEN (Woche 1.5 — Überblick reicht):
□ Playwright: data-testid Attribute in Blazor-Komponenten nutzen
□ Page Object Model: BasePage + konkrete Pages sauber trennen
□ API-Tests: Statuscodes UND fachliche Bedeutung prüfen
□ Audit-Log-Tests für MDR/Regulatorik schreiben
□ Dependency Injection statt ScenarioContext für komplexe Szenarien
□ CI/CD: Unit-Tests, Browser-Tests und Compliance-Tests getrennt
□ Testcontainers als nächsten Schritt nach InMemory kennen
```

### 11.9 Tageszusammenfassung

Heute hast du die wichtigsten Leitplanken für gute BDD- und Testautomatisierung im MedTech-Kontext gesammelt: lesbare Szenarien, klare Testschichten, sinnvolle Abstraktion und realistische Werkzeugeinschätzung.

Damit schließt der Kurs nicht nur mit neuen Techniken ab, sondern mit einem Bewertungsmaßstab: Du kannst künftig besser entscheiden, **welcher Testtyp wann sinnvoll ist** und woran du gute von fragilen Tests unterscheidest.

---

## 📚 Referenzen

| Ressource | Link |
|-----------|------|
| Reqnroll Dokumentation | https://docs.reqnroll.net |
| Reqnroll GitHub | https://github.com/reqnroll/Reqnroll |
| Playwright .NET | https://playwright.dev/dotnet |
| EF Core InMemory | https://learn.microsoft.com/ef/core/providers/in-memory |
| Testcontainers .NET | https://dotnet.testcontainers.org |
| Gherkin Referenz | https://cucumber.io/docs/gherkin/ |
| FluentAssertions | https://fluentassertions.com |
| Allure Reports | https://allurereport.org |

---

## 🏆 Was du nach 1,5 Wochen kannst

**Woche 1 — sicher beherrschen:**
- [ ] Feature-Files mit medizinischen Szenarien in Gherkin schreiben
- [ ] Reqnroll als Community-Fork von SpecFlow im Interview erklären
- [ ] Step Definitions in C# (Unit- und Integrationsebene)
- [ ] EF Core InMemory DB pro Szenario isoliert einsetzen — und Grenzen kennen
- [ ] Hooks für Setup/Teardown und Logging einrichten
- [ ] Scenario Outline für Dosierungsvalidierungen nutzen
- [ ] Data Tables für Laborwerte und Medikamentenlisten verarbeiten
- [ ] Test-Pyramide (70/20/10) erklären und anwenden

**Woche 1.5 — Grundverständnis, Überblick reicht:**
- [ ] Playwright-Tests für Blazor mit Page Object Model (Struktur verstehen)
- [ ] REST-API-Tests mit fachlich lesbaren Schritten schreiben
- [ ] Dependency Injection in Reqnroll nutzen statt ScenarioContext
- [ ] Tags für CI/CD-Filterung einsetzen (@smoke, @regulatorisch, @browser)
- [ ] Testcontainers als nächste Ausbaustufe nach InMemory kennen
- [ ] CI/CD mit Playwright und separaten Compliance-Reports aufsetzen
- [ ] Audit-Log-Tests für MDR/Regulatorik schreiben

---

*Viel Erfolg bei deiner neuen Stelle in MedTech! 🏥🚀*
*Gute Tests retten Leben — im wahrsten Sinne des Wortes.*
