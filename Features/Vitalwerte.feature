@vitalwerte
Feature: Vitalwert-Validierung
  Als behandelnder Arzt
  möchte ich Vitalwerte eines Patienten validieren
  damit kritische Werte sofort erkannt und behandelt werden

@smoke @sicherheit @regulatorisch @regression
  Scenario Outline: Vitalwert-Status bestimmen
    When der Vitalwert "<Vitalwert>" mit dem Messwert "<Messwert>" in "<Einheit>" erfasst wird
    Then sollte der klinische Status "<Erwarteter Status>" lauten

    Examples:
      | Vitalwert           | Messwert | Einheit | Erwarteter Status          |
      | Blutdruck           | 120/80   | mmHg    | NORMAL                     |
      | Blutdruck           | 180/110  | mmHg    | KRITISCH — Hypertoniekrise |
      | Puls                | 72       | bpm     | NORMAL                     |
      | Puls                | 145      | bpm     | WARNUNG — Tachykardie      |
      | Sauerstoffsättigung | 98       | %       | NORMAL                     |
      | Sauerstoffsättigung | 88       | %       | KRITISCH — Hypoxie         |
