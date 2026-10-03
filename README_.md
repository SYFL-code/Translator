English（英语）
Translator
[h3] Description [/h3]
The original author has not updated the [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]original mod[/url] for a long time, so this alternative version was made with some additions and adjustments.

This mod makes it easy for players to add translated names and descriptions (or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's built-in translator, so removing this mod restores the original names and descriptions.

[h3] Features [/h3]
- Manually add custom names / descriptions to any mod
- Batch export / import translations for all mods
- UI available in 10 languages (Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa)
- Only the **name and description** of mods are modified; this is not a general-purpose field translation tool

[h3] Usage [/h3]

[b]1. Quick button (single mod)[/b]
Click the "Rename" button in the top-right corner of the mod preview page.
In the opened temp.txt, edit:
  · Line 1 = translated name
  · Remaining lines = translated description (use <LINE> for line breaks)
Save and close the file, then click "Rename" again to apply.
If the name is empty, the translation is removed and the original text is restored.

[b]2. Batch translation (recommended)[/b]
In the mod settings page, click "Batch Translation" to generate ModRename_allMods.txt.
File format:

    [ModID]
    name=original name
    desc=original description
    trans_name=translated name
    trans_desc=translated description

Fill in trans_name / trans_desc, save and close the file,
then return to the settings page and click "Apply All".

[b]3. Manual index (by ID)[/b]
In the mod settings page, enter the target mod ID and click "Add Translation".
Edit temp.txt, save and close it, then click "Confirm Add".

[b]4. Copy someone else's translation file[/b]
Copy their save file to:
C:\Users\{YourUserName}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{Lang}.txt
{Lang} can be: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] How it works [/h3]
The displayed mod name and description are overridden through the game's built-in translator. The mod itself is not modified.
Removing this mod restores all changes.

User-defined translations are stored in:
- Runtime save: C:\Users\{UserName}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{Lang}.txt
- Bundled with the mod: <ModFolder>\text\text_{Lang}\strings.txt
- Automatic backup: <ModFolder>\backup\ModTranslatorSave_{Lang}_{Timestamp}.txt (max 20)
The old file `ly.ModRename_stringsSave.txt` is automatically migrated and merged on first launch.

[h3] Load Priority [/h3]
- To make this mod's translations **take priority** over other translation mods: place it **above** them.
- To only **supplement** what they do not cover: place it **below** them.
Due to the translation indexing mechanism, the last loaded translation wins.

[h3] Source [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


繁體中文（繁体中文）
翻譯器
[h3] 描述 [/h3]
由於原作者遲遲未更新 [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]原模組[/url]，
所以製作了此版本，增加與調整了部分內容。

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] 功能 [/h3]
- 手動為任意模組新增自訂名稱 / 簡介
- 批次匯出 / 匯入所有模組的翻譯
- 介面支援 11 種語言（中 / 繁 / 英 / 法 / 德 / 義 / 日 / 韓 / 葡 / 俄 / 西）
- 僅修改模組**名稱與簡介**，不是通用欄位翻譯工具

[h3] 使用方法 [/h3]

[b]1. 快捷按鈕（單一模組）[/b]
點擊模組預覽介面右上方的「重新命名」按鈕。
在開啟的 temp.txt 中按提示編輯：
  · 第一行 = 翻譯名稱
  · 其餘行 = 翻譯描述（可用 <LINE> 表示換行）
儲存並關閉檔案，回到遊戲再次點擊「重新命名」按鈕完成套用。
若名稱為空，則刪除該模組的翻譯並回復為原文。

[b]2. 批次翻譯（推薦）[/b]
在模組設定頁點擊「批次翻譯」，產生 ModRename_allMods.txt。
檔案格式：
    [ModID]
    name=原始名稱
    desc=原始描述
    trans_name=翻譯名稱
    trans_desc=翻譯描述
逐項填寫 trans_name / trans_desc 後儲存關閉，
回到模組設定頁點擊「套用全部」即可。

[b]3. 手動索引（依 ID 操作）[/b]
在模組設定頁輸入目標模組 ID，點擊「新增翻譯」。
按提示編輯 temp.txt，儲存關閉，然後點擊「確認新增」。

[b]4. 複製他人翻譯檔案[/b]
可將他人的存檔檔案複製到：
C:\Users\{你的使用者名稱}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{語言}.txt
{語言} 可選：Chi / Tra / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa。

[h3] 實作原理 [/h3]
透過遊戲內建翻譯器以「翻譯」的方式覆蓋模組顯示的名稱與描述，未對模組本身進行任何修改。
移除此模組即可還原所有變更。

使用者自訂翻譯儲存在：
- 執行存檔：C:\Users\{你的使用者名稱}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{語言}.txt
- 模組隨附翻譯：<模組目錄>\text\text_{語言}\strings.txt
- 自動備份：<模組目錄>\backup\ModTranslatorSave_{語言}_{時間戳記}.txt（最多保留 20 份）
舊版存檔 `ly.ModRename_stringsSave.txt` 會在首次啟動時自動遷移並合併。

[h3] 載入優先順序 [/h3]
- 若需本模組的翻譯[b]優先於[/b]其他翻譯模組：將本模組排在它們[b]之上[/b]。
- 若只想補充其他翻譯模組尚未涵蓋的內容：將本模組排在它們[b]之下[/b]。
由於翻譯索引機制的限制，遊戲將採用最後載入的那個翻譯。

[h3] 原始碼 [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


Français（法语）
Traducteur
[h3] Description [/h3]
L'auteur original n'ayant pas mis à jour le [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]mod original[/url] depuis longtemps, cette version alternative a été créée avec quelques modifications et ajouts.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] Fonctionnalités [/h3]
- Ajouter manuellement un nom / une description personnalisés à n'importe quel mod
- Exporter / importer les traductions de tous les mods en masse
- Interface disponible en 10 langues (chi / eng / fre / ger / ita / jap / kor / por / rus / spa)
- Modifie uniquement le **nom et la description** des mods, ce n'est pas un outil de traduction de champs générique

[h3] Utilisation [/h3]

[b]1. Bouton rapide (mod unique)[/b]
Cliquez sur le bouton « Renommer » en haut à droite de la page d'aperçu du mod.
Dans le fichier temp.txt ouvert, éditez :
  · Ligne 1 = nom traduit
  · Lignes suivantes = description traduite (utilisez <LINE> pour un saut de ligne)
Enregistrez et fermez, puis cliquez à nouveau sur « Renommer » pour appliquer.
Si le nom est vide, la traduction est supprimée et le texte original restauré.

[b]2. Traduction en masse (recommandé)[/b]
Dans la page des paramètres du mod, cliquez sur « Traduction en masse » pour générer ModRename_allMods.txt.
Format :

    [ModID]
    name=nom original
    desc=description originale
    trans_name=nom traduit
    trans_desc=description traduite

Remplissez trans_name / trans_desc puis enregistrez,
revenez à la page des paramètres et cliquez sur « Tout appliquer ».

[b]3. Indexation manuelle (par ID)[/b]
Dans la page des paramètres, saisissez l'ID du mod cible, cliquez sur « Ajouter la traduction ».
Éditez le temp.txt, enregistrez et fermez, puis cliquez sur « Confirmer l'ajout ».

[b]4. Copier un fichier de traduction[/b]
Copiez le fichier de sauvegarde à l'emplacement :
C:\Users\{votre nom d'utilisateur}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{langue}.txt
{langue} peut être : Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] Fonctionnement [/h3]
Les noms et descriptions affichés sont remplacés via le traducteur intégré du jeu ; le mod lui-même n'est pas modifié.
Supprimer ce mod restaure tout.

Traductions personnalisées enregistrées dans :
- Sauvegarde : C:\Users\{nom}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{langue}.txt
- Fournies avec le mod : <dossier du mod>\text\text_{langue}\strings.txt
- Sauvegardes auto : <dossier du mod>\backup\ModTranslatorSave_{langue}_{horodatage}.txt (max 20)
L'ancien fichier `ly.ModRename_stringsSave.txt` est migré et fusionné automatiquement au premier lancement.

[h3] Priorité de chargement [/h3]
- Pour que ce mod soit **prioritaire** sur les autres mods de traduction : placez-le **au-dessus** d'eux.
- Pour seulement **compléter** ce qu'ils ne couvrent pas : placez-le **en dessous**.
En raison de l'indexation des traductions, c'est la dernière traduction chargée qui est utilisée.

[h3] Source [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


Deutsch（德语）
Übersetzer
[h3] Beschreibung [/h3]
Da der Originalautor das [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]Originalmod[/url] lange nicht aktualisiert hat, wurde diese Alternativversion mit einigen Änderungen und Ergänzungen erstellt.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] Funktionen [/h3]
- Manuell eigene Namen / Beschreibungen für beliebige Mods hinzufügen
- Übersetzungen aller Mods stapelweise exportieren / importieren
- Oberfläche in 10 Sprachen (chi / eng / fre / ger / ita / jap / kor / por / rus / spa)
- Ändert nur **Name und Beschreibung** von Mods, kein allgemeines Feldübersetzungswerkzeug

[h3] Verwendung [/h3]

[b]1. Schnellbutton (einzelner Mod)[/b]
Klicke in der Mod-Vorschau oben rechts auf „Umbenennen".
In der geöffneten temp.txt bearbeiten:
  · Zeile 1 = übersetzter Name
  · Restliche Zeilen = übersetzte Beschreibung (mit <LINE> umbrechen)
Speichern und schließen, dann erneut auf „Umbenennen" klicken.
Ist der Name leer, wird die Übersetzung entfernt und der Originaltext wiederhergestellt.

[b]2. Stapelübersetzung (empfohlen)[/b]
In den Mod-Einstellungen auf „Stapelübersetzung" klicken — ModRename_allMods.txt wird erzeugt.
Format:

    [ModID]
    name=ursprünglicher Name
    desc=ursprüngliche Beschreibung
    trans_name=übersetzter Name
    trans_desc=übersetzte Beschreibung

trans_name / trans_desc ausfüllen, speichern,
zurück zu den Einstellungen und auf „Alle anwenden" klicken.

[b]3. Manuelle Indexierung (per ID)[/b]
In den Einstellungen die Ziel-Mod-ID eingeben, auf „Übersetzung hinzufügen" klicken.
temp.txt bearbeiten, speichern, schließen, dann auf „Hinzufügen bestätigen" klicken.

[b]4. Übersetzungsdatei kopieren[/b]
Speicherdatei kopieren nach:
C:\Users\{Benutzername}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{Sprache}.txt
{Sprache}: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] Funktionsweise [/h3]
Name und Beschreibung werden über den integrierten Übersetzer des Spiels ersetzt; der Mod selbst wird nicht verändert.
Diesen Mod entfernen stellt alles wieder her.

Eigene Übersetzungen liegen in:
- Speicher: C:\Users\{Benutzer}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{Sprache}.txt
- Mitgeliefert: <Modordner>\text\text_{Sprache}\strings.txt
- Auto-Backup: <Modordner>\backup\ModTranslatorSave_{Sprache}_{Zeitstempel}.txt (max. 20)
Die alte Datei `ly.ModRename_stringsSave.txt` wird beim ersten Start automatisch migriert und zusammengeführt.

[h3] Ladepriorität [/h3]
- Damit dieser Mod **Vorrang** vor anderen Übersetzungsmods hat: **über** sie stellen.
- Nur zum **Ergänzen**: **unter** sie stellen.
Wegen der Übersetzungsindizierung gewinnt die zuletzt geladene Übersetzung.

[h3] Quelle [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


Italiano（意大利语）
Traduttore
[h3] Descrizione [/h3]
Poiché l'autore originale non aggiorna da tempo il [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]mod originale[/url], è stata creata questa versione alternativa con alcune modifiche e aggiunte.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] Funzionalità [/h3]
- Aggiungere manualmente nome / descrizione personalizzati a qualsiasi mod
- Esportare / importare in blocco le traduzioni di tutti i mod
- Interfaccia in 10 lingue (chi / eng / fre / ger / ita / jap / kor / por / rus / spa)
- Modifica solo **nome e descrizione** dei mod, non è uno strumento generico di traduzione campi

[h3] Utilizzo [/h3]

[b]1. Pulsante rapido (singolo mod)[/b]
Clicca su "Rinomina" in alto a destra nella pagina di anteprima del mod.
Nel temp.txt aperto modifica:
  · Riga 1 = nome tradotto
  · Righe successive = descrizione tradotta (usa <LINE> per andare a capo)
Salva e chiudi, poi clicca di nuovo su "Rinomina" per applicare.
Se il nome è vuoto, la traduzione viene rimossa e torna l'originale.

[b]2. Traduzione in blocco (consigliato)[/b]
Nella pagina delle impostazioni del mod clicca su "Traduzione in blocco" per generare ModRename_allMods.txt.
Formato:

    [ModID]
    name=nome originale
    desc=descrizione originale
    trans_name=nome tradotto
    trans_desc=descrizione tradotta

Compila trans_name / trans_desc, salva,
torna alle impostazioni e clicca su "Applica tutto".

[b]3. Indicizzazione manuale (per ID)[/b]
Nelle impostazioni inserisci l'ID del mod, clicca su "Aggiungi traduzione".
Modifica temp.txt, salva, chiudi, poi clicca su "Conferma aggiunta".

[b]4. Copiare un file di traduzione[/b]
Copia il file di salvataggio in:
C:\Users\{nome utente}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{lingua}.txt
{lingua}: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] Come funziona [/h3]
Nome e descrizione vengono sostituiti tramite il traduttore integrato del gioco; il mod non viene modificato.
Rimuovendo questo mod si ripristina tutto.

Traduzioni personalizzate salvate in:
- Salvataggio: C:\Users\{utente}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{lingua}.txt
- Incluse nel mod: <cartella mod>\text\text_{lingua}\strings.txt
- Backup auto: <cartella mod>\backup\ModTranslatorSave_{lingua}_{timestamp}.txt (max 20)
Il vecchio file `ly.ModRename_stringsSave.txt` viene migrato e unito automaticamente al primo avvio.

[h3] Priorità di caricamento [/h3]
- Per far **prevalere** questo mod sugli altri mod di traduzione: mettilo **sopra** di essi.
- Per **integrarli** soltanto: mettilo **sotto** di essi.
Per via dell'indicizzazione, vince la traduzione caricata per ultima.

[h3] Sorgente [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


日本語（日语）
翻訳ツール
[h3] 説明 [/h3]
原作者が[url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]元のMOD[/url]を長らく更新していないため、一部内容を追加・調整した代替版を作成しました。

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] 機能 [/h3]
- 任意のMODに手動でカスタム名 / 説明を追加
- すべてのMODの翻訳を一括エクスポート / インポート
- UIは10言語対応（中 / 英 / 仏 / 独 / 伊 / 日 / 韓 / 葡 / 露 / 西）
- 変更できるのはMODの**名前と説明のみ**。汎用フィールド翻訳ツールではありません

[h3] 使い方 [/h3]

[b]1. クイックボタン（単一MOD）[/b]
MODプレビュー画面右上の「名前を変更」ボタンをクリック。
開いたtemp.txtを編集：
  · 1行目 = 翻訳名
  · 2行目以降 = 翻訳説明（改行は <LINE>）
保存して閉じ、もう一度「名前を変更」をクリックして適用。
名前を空にすると翻訳が削除され、原文に戻ります。

[b]2. 一括翻訳（推奨）[/b]
MOD設定ページで「一括翻訳」をクリック → ModRename_allMods.txt が生成されます。
フォーマット：

    [ModID]
    name=元の名前
    desc=元の説明
    trans_name=翻訳名
    trans_desc=翻訳説明

trans_name / trans_desc を記入して保存し、
設定ページに戻って「すべて適用」をクリック。

[b]3. 手動インデックス（ID指定）[/b]
設定ページで対象MODのIDを入力し、「翻訳を追加」をクリック。
temp.txtを編集して保存・閉じた後、「追加を確定」をクリック。

[b]4. 他の人の翻訳ファイルをコピー[/b]
セーブファイルを以下にコピー：
C:\Users\{ユーザー名}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{言語}.txt
{言語}：Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa。

[h3] 仕組み [/h3]
ゲーム内蔵の翻訳機能でMODの表示名と説明を上書きします。MOD本体は変更しません。
このMODを削除すればすべて元に戻ります。

カスタム翻訳の保存先：
- セーブ：C:\Users\{ユーザー}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{言語}.txt
- MOD同梱：<MODフォルダ>\text\text_{言語}\strings.txt
- 自動バックアップ：<MODフォルダ>\backup\ModTranslatorSave_{言語}_{タイムスタンプ}.txt（最大20件）
旧ファイル `ly.ModRename_stringsSave.txt` は初回起動時に自動で移行・統合されます。

[h3] 読み込み優先度 [/h3]
- このMODの翻訳を**優先**したい場合：他の翻訳MODより**上**に配置。
- **補完のみ**したい場合：他の翻訳MODより**下**に配置。
翻訳インデックスの仕様により、最後に読み込まれた翻訳が採用されます。

[h3] ソース [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


한국어（韩语）
번역기
[h3] 설명 [/h3]
원작자가 [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]원본 모드[/url]를 오랫동안 업데이트하지 않아, 일부를 수정·추가한 대체 버전을 제작했습니다.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] 기능 [/h3]
- 모든 모드에 사용자 지정 이름 / 설명을 수동으로 추가
- 모든 모드의 번역을 일괄 내보내기 / 가져오기
- UI는 10개 언어 지원 (중 / 영 / 프 / 독 / 이 / 일 / 한 / 포 / 러 / 스)
- 모드의 **이름과 설명만** 수정합니다. 범용 필드 번역 도구가 아닙니다.

[h3] 사용 방법 [/h3]

[b]1. 빠른 버튼 (단일 모드)[/b]
모드 미리보기 페이지 오른쪽 위의 "이름 변경" 버튼을 클릭합니다.
열린 temp.txt를 편집하세요:
  · 첫 번째 줄 = 번역 이름
  · 나머지 줄 = 번역 설명 (줄바꿈은 <LINE>)
저장하고 닫은 뒤, 다시 "이름 변경"을 클릭해 적용합니다.
이름을 비우면 해당 번역이 삭제되고 원문으로 되돌아갑니다.

[b]2. 일괄 번역 (권장)[/b]
모드 설정 페이지에서 "일괄 번역"을 클릭하면 ModRename_allMods.txt가 생성됩니다.
형식:

    [ModID]
    name=원래 이름
    desc=원래 설명
    trans_name=번역 이름
    trans_desc=번역 설명

trans_name / trans_desc를 입력하고 저장한 뒤,
설정 페이지로 돌아가 "모두 적용"을 클릭하세요.

[b]3. 수동 인덱스 (ID로)[/b]
설정 페이지에서 대상 모드 ID를 입력하고 "번역 추가"를 클릭합니다.
temp.txt를 편집·저장·닫은 뒤 "추가 확인"을 클릭하세요.

[b]4. 다른 사람의 번역 파일 복사[/b]
세이브 파일을 다음 위치로 복사하세요:
C:\Users\{사용자 이름}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{언어}.txt
{언어}: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] 동작 원리 [/h3]
게임 내장 번역기로 모드의 표시 이름과 설명을 덮어씁니다. 모드 자체는 수정하지 않습니다.
이 모드를 제거하면 모든 변경이 원래대로 돌아갑니다.

사용자 지정 번역 저장 위치:
- 세이브: C:\Users\{사용자}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{언어}.txt
- 모드 동봉: <모드 폴더>\text\text_{언어}\strings.txt
- 자동 백업: <모드 폴더>\backup\ModTranslatorSave_{언어}_{타임스탬프}.txt (최대 20개)
구버전 파일 `ly.ModRename_stringsSave.txt`는 첫 실행 시 자동으로 이전·통합됩니다.

[h3] 로드 우선순위 [/h3]
- 이 모드의 번역을 **우선**하려면: 다른 번역 모드보다 **위**에 배치.
- 단지 **보완**만 하려면: 다른 번역 모드보다 **아래**에 배치.
번역 인덱스 특성상 마지막에 로드된 번역이 채택됩니다.

[h3] 소스 [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


Português（葡萄牙语）
Tradutor
[h3] Descrição [/h3]
Como o autor original não atualiza o [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]mod original[/url] há muito tempo, foi criada esta versão alternativa com alguns ajustes e adições.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] Recursos [/h3]
- Adicionar manualmente nome / descrição personalizados a qualquer mod
- Exportar / importar em lote as traduções de todos os mods
- Interface em 10 idiomas (chi / eng / fre / ger / ita / jap / kor / por / rus / spa)
- Modifica apenas o **nome e a descrição** dos mods, não é uma ferramenta genérica de tradução de campos

[h3] Como usar [/h3]

[b]1. Botão rápido (mod único)[/b]
Clique em "Renomear" no canto superior direito da pré-visualização do mod.
No temp.txt aberto, edite:
  · Linha 1 = nome traduzido
  · Linhas seguintes = descrição traduzida (use <LINE> para quebrar linha)
Salve e feche, depois clique novamente em "Renomear" para aplicar.
Se o nome estiver vazio, a tradução é removida e o original restaurado.

[b]2. Tradução em lote (recomendado)[/b]
Na página de configurações do mod, clique em "Tradução em lote" para gerar ModRename_allMods.txt.
Formato:

    [ModID]
    name=nome original
    desc=descrição original
    trans_name=nome traduzido
    trans_desc=descrição traduzida

Preencha trans_name / trans_desc, salve,
volte às configurações e clique em "Aplicar tudo".

[b]3. Indexação manual (por ID)[/b]
Nas configurações, insira o ID do mod alvo e clique em "Adicionar tradução".
Edite o temp.txt, salve, feche, depois clique em "Confirmar adição".

[b]4. Copiar arquivo de tradução[/b]
Copie o arquivo de save para:
C:\Users\{nome de usuário}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{idioma}.txt
{idioma}: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] Como funciona [/h3]
Nome e descrição exibidos são substituídos pelo tradutor interno do jogo; o mod em si não é alterado.
Remover este mod restaura tudo.

Traduções personalizadas salvas em:
- Save: C:\Users\{usuário}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{idioma}.txt
- Junto ao mod: <pasta do mod>\text\text_{idioma}\strings.txt
- Backup automático: <pasta do mod>\backup\ModTranslatorSave_{idioma}_{timestamp}.txt (máx. 20)
O arquivo antigo `ly.ModRename_stringsSave.txt` é migrado e mesclado automaticamente no primeiro início.

[h3] Prioridade de carregamento [/h3]
- Para este mod ter **prioridade** sobre outros mods de tradução: coloque-o **acima** deles.
- Para apenas **complementar**: coloque-o **abaixo** deles.
Devido à indexação de traduções, vale a última tradução carregada.

[h3] Código-fonte [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


Русский（俄语）
Переводчик
[h3] Описание [/h3]
Поскольку автор оригинала давно не обновлял [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]оригинальный мод[/url], была сделана эта альтернативная версия с некоторыми изменениями и дополнениями.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] Возможности [/h3]
- Вручную добавлять свои названия / описания к любому моду
- Массовый экспорт / импорт переводов всех модов
- Интерфейс на 10 языках (кит / англ / фр / нем / ит / яп / кор / порт / рус / исп)
- Изменяет только **название и описание** мода, это не универсальный переводчик полей

[h3] Как пользоваться [/h3]

[b]1. Быстрая кнопка (один мод)[/b]
Нажмите «Переименовать» в правом верхнем углу страницы мода.
В открывшемся temp.txt отредактируйте:
  · 1-я строка = переведённое название
  · Остальные строки = переведённое описание (перенос строки — <LINE>)
Сохраните и закройте, затем снова нажмите «Переименовать».
Если название пустое — перевод удаляется и возвращается оригинал.

[b]2. Пакетный перевод (рекомендуется)[/b]
В настройках мода нажмите «Пакетный перевод» — создастся ModRename_allMods.txt.
Формат:

    [ModID]
    name=исходное название
    desc=исходное описание
    trans_name=переведённое название
    trans_desc=переведённое описание

Заполните trans_name / trans_desc, сохраните,
вернитесь в настройки и нажмите «Применить всё».

[b]3. Ручная индексация (по ID)[/b]
В настройках введите ID нужного мода, нажмите «Добавить перевод».
Отредактируйте temp.txt, сохраните, закройте, затем нажмите «Подтвердить добавление».

[b]4. Копирование чужого файла перевода[/b]
Скопируйте файл сохранения в:
C:\Users\{имя пользователя}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{язык}.txt
{язык}: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] Как это работает [/h3]
Название и описание подменяются через встроенный переводчик игры; сам мод не изменяется.
Удаление этого мода возвращает всё обратно.

Пользовательские переводы хранятся в:
- Сохранение: C:\Users\{пользователь}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{язык}.txt
- В комплекте с модом: <папка мода>\text\text_{язык}\strings.txt
- Автобэкап: <папка мода>\backup\ModTranslatorSave_{язык}_{время}.txt (до 20 шт.)
Старый файл `ly.ModRename_stringsSave.txt` автоматически переносится и объединяется при первом запуске.

[h3] Приоритет загрузки [/h3]
- Чтобы этот мод имел **приоритет** над другими модами перевода: поставьте его **выше**.
- Чтобы только **дополнять** их: поставьте его **ниже**.
Из-за индексации переводов побеждает тот, что загружен последним.

[h3] Исходный код [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]


Español（西班牙语）
Traductor
[h3] Descripción [/h3]
Como el autor original no ha actualizado el [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3759456473]mod original[/url] desde hace tiempo, se creó esta versión alternativa con algunos ajustes y añadidos.

This is a mod that makes it easy for players to add translated names and descriptions
(or notes) to installed mods. It does not modify the mods themselves; it uses Rain World's
built-in translator, so removing this mod restores the original names and descriptions.

[h3] Funciones [/h3]
- Añadir manualmente nombre / descripción personalizados a cualquier mod
- Exportar / importar en lote las traducciones de todos los mods
- Interfaz en 10 idiomas (chi / eng / fre / ger / ita / jap / kor / por / rus / spa)
- Solo modifica el **nombre y la descripción** de los mods; no es una herramienta genérica de traducción de campos

[h3] Cómo usar [/h3]

[b]1. Botón rápido (mod individual)[/b]
Haz clic en "Renombrar" en la esquina superior derecha de la vista previa del mod.
En el temp.txt abierto edita:
  · Línea 1 = nombre traducido
  · Líneas restantes = descripción traducida (usa <LINE> para saltos de línea)
Guarda y cierra, luego vuelve a hacer clic en "Renombrar" para aplicar.
Si el nombre queda vacío, la traducción se elimina y se restaura el original.

[b]2. Traducción en lote (recomendado)[/b]
En la página de ajustes del mod haz clic en "Traducción en lote" — se genera ModRename_allMods.txt.
Formato:

    [ModID]
    name=nombre original
    desc=descripción original
    trans_name=nombre traducido
    trans_desc=descripción traducida

Rellena trans_name / trans_desc, guarda,
vuelve a los ajustes y haz clic en "Aplicar todo".

[b]3. Indexación manual (por ID)[/b]
En los ajustes introduce el ID del mod objetivo y haz clic en "Añadir traducción".
Edita temp.txt, guarda, cierra y luego haz clic en "Confirmar adición".

[b]4. Copiar un archivo de traducción[/b]
Copia el archivo de guardado a:
C:\Users\{tu usuario}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{idioma}.txt
{idioma}: Chi / Eng / Fre / Ger / Ita / Jap / Kor / Por / Rus / Spa.

[h3] Cómo funciona [/h3]
El nombre y la descripción mostrados se sustituyen mediante el traductor integrado del juego; el mod en sí no se modifica.
Eliminar este mod restaura todo.

Traducciones personalizadas guardadas en:
- Guardado: C:\Users\{usuario}\AppData\LocalLow\Videocult\Rain World\ModConfigs\ModTranslatorSave_{idioma}.txt
- Incluidas con el mod: <carpeta del mod>\text\text_{idioma}\strings.txt
- Copia de seguridad automática: <carpeta del mod>\backup\ModTranslatorSave_{idioma}_{marca de tiempo}.txt (máx. 20)
El archivo antiguo `ly.ModRename_stringsSave.txt` se migra y combina automáticamente en el primer inicio.

[h3] Prioridad de carga [/h3]
- Para que este mod tenga **prioridad** sobre otros mods de traducción: colócalo **encima** de ellos.
- Para solo **complementar**: colócalo **debajo** de ellos.
Por el mecanismo de índice de traducciones, gana la última traducción cargada.

[h3] Código fuente [/h3]
[url=https://github.com/SYFL-code/Translator]GitHub[/url]