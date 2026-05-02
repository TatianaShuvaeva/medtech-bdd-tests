@auditlog @regulatorisch @integration
Feature: Audit-Log bei Rezeptausstellung
  Als Compliance-Officer
  möchte ich sicherstellen, dass jede Rezeptausstellung im Audit-Log protokolliert wird
  damit MDR-Anforderungen zur Nachvollziehbarkeit erfüllt werden

 
  @audit-relevant @smoke
  Scenario: Audit-Log wird bei Rezeptausstellung erstellt
    Given Arzt "Dr. Schmidt" mit Lizenznummer "BAY-12345" ist im System angemeldet
    And Patient "Thomas Braun" (ID: "P-7823") ist geöffnet
    When der aktuelle Arzt "Metformin 500mg" einmal täglich verschreibt
    Then sollte das Rezept gespeichert werden
    And das Audit-Log sollte einen Eintrag enthalten:
      | Feld      | Wert            |
      | Aktion       | REZEPT_ERSTELLT |
      | Benutzer     | Dr. Schmidt     |
      | Lizenznummer | BAY-12345       |
      | EntityTyp    | Rezept          |
