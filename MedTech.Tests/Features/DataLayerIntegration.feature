@integration @datalayer
Feature: Data Layer – Datenpersistenz im Backend
  Als Entwickler
  möchte ich sicherstellen, dass Daten korrekt in der Datenbank gespeichert werden
  damit die Datenschicht unabhängig von der UI korrekt funktioniert

  @smoke @datalayer
  Scenario: Rezept wird korrekt in der Datenbank persistiert
    Given Arzt "Dr. Weber" mit Lizenznummer "MUC-999" ist im System angemeldet
    And Patient "Lisa Berger" (ID: "P-100") ist geöffnet
    When der aktuelle Arzt "Ibuprofen 400mg" verschreibt
    Then sollte das Rezept in der Datenbank persistiert sein
    And das persistierte Rezept sollte folgende Felder enthalten:
      | Feld       | Wert            |
      | Medikament | Ibuprofen 400mg |

  @smoke @datalayer @audit
  Scenario: Audit-Log-Eintrag wird direkt in der Datenbank gespeichert
    Given Arzt "Dr. Klein" mit Lizenznummer "BER-123" ist im System angemeldet
    And Patient "Hans Groß" (ID: "P-101") ist geöffnet
    When der aktuelle Arzt "Metformin 500mg" verschreibt
    Then sollte das Audit-Log direkt in der DB einen Eintrag mit Aktion "REZEPT_ERSTELLT" enthalten
    And der Audit-Log-Eintrag sollte Benutzer "Dr. Klein" mit Lizenznummer "BER-123" protokolliert haben

  @datalayer @negativ
  Scenario: Bei Allergie wird kein Rezept in der Datenbank gespeichert
    Given Arzt "Dr. Müller" mit Lizenznummer "HAM-456" ist im System angemeldet
    And Patient "Anna Böhm" (ID: "P-102") ist geöffnet
    And Patient "Anna Böhm" hat eine dokumentierte Allergie gegen "Penicillin"
    When der aktuelle Arzt versucht "Amoxicillin 500mg" zu verschreiben
    Then sollte kein Rezept in der Datenbank persistiert sein
    And sollte kein Audit-Log-Eintrag in der Datenbank vorhanden sein
