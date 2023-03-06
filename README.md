# DeepTimer

這是一個專為 AWS DeepRacer 設計的自動計時器， 需要連接專門 Arduino FSR 壓力感應器使用。

This is an automatic timer specially designed for AWS DeepRacer, which needs to be connected to a dedicated Arduino FSR sensor.
 
## 使用方法

### 輸入隊伍資料
    建立一個 Excel 文件, 含 2 個列: Team 和 Name。 其中 Team 為編號, Name 為隊名。
    Create an Excel file with 2 columns: Team and Name. Where "Team" is the number and "Name" is the team name.

### 連接 Arduino 裝置
    請打開相關 COM Port， 注意需要安裝 Arduino USB driver。成功連接后可以用 Monitor 去觀察是否有資料回傳。
    Please open the COM Port which is arduino device used, and note that the Arduino USB driver needs to be installed first. After connect the sensor you can use Monitor to observe whether the data returned.

### 計時器畫面
    完成以上 2 個步驟才能打開 lap timer 畫面，背景可自由更換。
    After completing the above 2 steps, the lap timer screen can be opened, and the background can be changed freely.
 
- #### 開始計時 Start
    選擇一支隊伍，然后按開始。計時器會立即進行倒數。 當車子第一次經過起跑線才開始正式計時，當再次經過起跑線時叫作完成一圈。
    Select a team and press start. The timer counts down immediately. When the car passes the starting line first time, the lap timing starts, and when it passes the starting line again, it is called to complete a lap.

- #### 重置計時 Reset
    倒數結束或暫停后才能重置時間。  
    The timer cannot be reset until the countdown ends or is paused.
    
### 測試模式
    勾選 Test mode 后不會記錄時間到資料庫，只作顯示時間之用。
    After checking the Test mode, the time will not be recorded in the database, it is only used for displaying the time.

### 滙出Excel
    把比賽隊伍時間儲存到 Excel 文件中.
    Export the times data to an Excel file.

### 顯示時間排名
- #### 運行排名服務器
    輸入主機 IP 地址。
    Enter the host IP address.
    
- #### DeepLive
    在另一台電腦上運行 DeepLive，連接服務器。
    Run DeepLive on another computer and connect to the server.
