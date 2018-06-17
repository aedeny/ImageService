package com.potatos.two.imageserviceandroid;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.NetworkRequest;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.support.annotation.RequiresApi;
import android.support.v4.app.NotificationCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import static android.net.NetworkCapabilities.NET_CAPABILITY_INTERNET;

public class MainActivity extends AppCompatActivity {

    private BroadcastReceiver mBroadcastReceiver;
    private TcpClient mTcpClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        final Context context = getApplicationContext();

        final Button startServiceButton = findViewById(R.id.startServiceButton);
        final Button stopServiceButton = findViewById(R.id.stopServiceButton);
        stopServiceButton.setEnabled(false);
        startServiceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startServiceButton.setEnabled(false);
                stopServiceButton.setEnabled(true);
                CharSequence text = "Service started...";
                int duration = Toast.LENGTH_SHORT;
                Toast toast = Toast.makeText(context, text, duration);
                toast.show();
            }
        });
        stopServiceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startServiceButton.setEnabled(true);
                stopServiceButton.setEnabled(false);
                CharSequence text = "Service stopped";
                int duration = Toast.LENGTH_SHORT;
                Toast toast = Toast.makeText(context, text, duration);
                toast.show();
            }
        });

        final IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        intentFilter.addAction("android.net.wifi.STATE_CHANGE");

        NetworkRequest.Builder request = new NetworkRequest.Builder();
        request.addCapability(NET_CAPABILITY_INTERNET);

        mBroadcastReceiver = new BroadcastReceiver() {
            @RequiresApi(api = Build.VERSION_CODES.O)
            @Override
            public void onReceive(Context context, Intent intent) {
                context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
                NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);

                final NotificationManager notificationManager = (NotificationManager) context.getSystemService
                        (Context.NOTIFICATION_SERVICE);
                NotificationChannel channel = new NotificationChannel("default", "Channelname",
                        NotificationManager.IMPORTANCE_DEFAULT);
                channel.setDescription("Channel description");
                if (notificationManager != null) {
                    notificationManager.createNotificationChannel(channel);
                }
                final NotificationCompat.Builder builder = new NotificationCompat.Builder(context, "default");
                builder.setSmallIcon(R.drawable.ic_launcher_background);
                builder.setContentTitle("Transferring Images status");
                builder.setContentText("In progress");


                if (networkInfo != null) {
                    if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
                        if (networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                            mTcpClient.connect(notificationManager, builder);
                        }
                    }
                }
            }
        };

    }
}
