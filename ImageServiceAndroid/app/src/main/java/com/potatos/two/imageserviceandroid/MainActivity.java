package com.potatos.two.imageserviceandroid;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;

public class MainActivity extends AppCompatActivity {


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        getApplicationContext();

        final Button startServiceButton = findViewById(R.id.startServiceButton);
        final Button stopServiceButton = findViewById(R.id.stopServiceButton);
        askForPermissions();
        stopServiceButton.setEnabled(false);
        startServiceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startServiceButton.setEnabled(false);
                stopServiceButton.setEnabled(true);
                startService(view);
            }
        });

        stopServiceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                startServiceButton.setEnabled(true);
                stopServiceButton.setEnabled(false);
                stopService(view);
            }
        });
    }

    private void startService(View view) {
        Intent intent = new Intent(this, ImageService.class);
        startService(intent);
    }

    private void stopService(View view) {
        Intent intent = new Intent(this, ImageService.class);
        stopService(intent);
    }

    private void askForPermissions() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_EXTERNAL_STORAGE) !=
                PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission
                    .READ_EXTERNAL_STORAGE}, 1);
        }
    }
}

