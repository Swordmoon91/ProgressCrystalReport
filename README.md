# Crystal Report Console

Applicazione console per l'esecuzione e l'esportazione di report Crystal Reports.

## Funzionalità

- Caricamento di report Crystal Reports (.rpt)
- Connessione a database tramite DSN ODBC
- Supporto per parametri del report
- Caricamento parametri da file
- Esportazione in vari formati (PDF, Word, Excel, HTML, CSV, RTF)
- Logging dettagliato delle operazioni

## Requisiti

- .NET Framework (versione compatibile con Crystal Reports)
- Crystal Reports Runtime
- Librerie Crystal Reports installate
- DSN ODBC configurato per la connessione al database

## Utilizzo

```
ProgressCrystalReport.exe [opzioni]
```

### Opzioni

- `-r <percorso>` - Percorso completo del file report (.rpt) [OBBLIGATORIO]
- `-d <nome>` - Nome DSN ODBC [OBBLIGATORIO]
- `-u <username>` - Username per la connessione ODBC
- `-p <password>` - Password per la connessione ODBC
- `-o <percorso>` - Percorso di output per il report esportato
- `-P:<nome> <valore>` - Parametro per il report (es. -P:CustomerID 12345)
- `-f <file>` - File di parametri separati da CHAR(1)
- `-h, --help` - Mostra il messaggio di aiuto

### Esempi

```
ProgressCrystalReport.exe -r "C:\report.rpt" -d "MioDSN" -u "admin" -p "password" -o "C:\output.pdf" -P:Data "2023-01-01"
ProgressCrystalReport.exe -r "C:\report.rpt" -d "MioDSN" -f "parametri.txt"
```

## File di parametri

Il file di parametri deve contenere i valori dei parametri separati dal carattere ASCII 1 (CHAR(1)).

## Configurazione

Nel file `App.config` è possibile impostare valori predefiniti per:

- `DefaultReportPath` - Percorso predefinito del report
- `DefaultOdbcDsn` - Nome DSN ODBC predefinito
- `DefaultOutputPath` - Percorso di output predefinito

## Logging

I log dell'applicazione vengono salvati nella cartella `logs`. La configurazione del logging può essere personalizzata nel file `log4net.config`.

## Struttura del Progetto

- **Program.cs**: Punto di ingresso dell'applicazione
- **Services/**
  - **CommandLineParser.cs**: Gestione dei parametri della riga di comando
  - **ReportExecutor.cs**: Logica di esecuzione dei report
  - **ParameterManager.cs**: Gestione dei parametri dei report
  - **ReportExporter.cs**: Funzionalità di esportazione
- **Models/**
  - **ApplicationParameters.cs**: Modello per i parametri dell'applicazione
  - **ConnectionConfig.cs**: Configurazione della connessione
  - **ReportParameter.cs**: Modello per i parametri del report
- **Utils/**
  - **LogHelper.cs**: Utilità per il logging
  - **FileHelper.cs**: Utilità per la gestione dei file
  - **ParameterTypeConverter.cs**: Conversione dei tipi di parametri
