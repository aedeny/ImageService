package com.potatos.two.imageserviceandroid;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.support.annotation.RequiresApi;
import android.support.v4.app.NotificationCompat;
import android.widget.Toast;

public class ImageService extends Service {

    private TcpClient mTcpClient;

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    public void onCreate() {
        super.onCreate();
        mTcpClient = new TcpClient();
    }

    @Override
    public int onStartCommand(Intent intent, int flag, int startId) {

        final IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        intentFilter.addAction("android.net.wifi.STATE_CHANGE");
        BroadcastReceiver broadcastReceiver = new BroadcastReceiver() {
            @RequiresApi(api = Build.VERSION_CODES.O)
            @Override
            public void onReceive(Context context, Intent intent) {
                context.getSystemService(Context.WIFI_SERVICE);
                NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);

                final NotificationManager notificationManager = (NotificationManager) context
                        .getSystemService(Context.NOTIFICATION_SERVICE);
                NotificationChannel channel = new NotificationChannel("default", "Channel name",
                        NotificationManager.IMPORTANCE_DEFAULT);
                channel.setDescription("Channel description");

                if (notificationManager != null) {
                    notificationManager.createNotificationChannel(channel);
                }

                final NotificationCompat.Builder builder = new NotificationCompat.Builder
                        (context, "default");
                builder.setSmallIcon(R.drawable.ic_launcher_background);
                builder.setContentTitle("Transferring Images status");
                builder.setContentText("In progress");


                if (networkInfo != null && networkInfo.getType() == ConnectivityManager.TYPE_WIFI
                        && networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                    Toast.makeText(context, "Connected to WiFi", Toast.LENGTH_SHORT).show();
                    mTcpClient.connect(notificationManager, builder);
                }
            }
        };

        registerReceiver(broadcastReceiver, intentFilter);
        Toast.makeText(this, "Service starting...", Toast.LENGTH_SHORT).show();
        return START_STICKY;
    }

    public void onDestroy() {
        Toast.makeText(this, "Service stopping...", Toast.LENGTH_SHORT).show();
    }
}