package com.potatos.two.imageserviceandroid;

import android.app.NotificationManager;
import android.os.Environment;
import android.support.v4.app.NotificationCompat;
import android.util.Log;

import java.io.DataOutputStream;
import java.io.File;
import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.file.Files;
import java.util.Arrays;
import java.util.List;

class TcpClient {
    private static final String mIPAddress = "10.0.2.2";
    private static final int mPort = 3748;
    private DataOutputStream mOutputStream;


    public void connect(final NotificationManager nm, final NotificationCompat
            .Builder builder) {
        Thread thread = new Thread(new Runnable() {
            @Override
            public void run() {
                getConnection();
                File dcim;
                try {
                    dcim = new File(Environment.getExternalStoragePublicDirectory(Environment
                            .DIRECTORY_DCIM), "Camera");
                } catch (NullPointerException e) {
                    System.out.println(e.getMessage());
                    return;
                }
                File[] files = dcim.listFiles();
                if (files == null) {
                    return;
                }
                List<File> imageFiles = Arrays.asList(dcim.listFiles());
                int numberOfImages = imageFiles.size();
                int transferredImages = 0;

                try {
                    mOutputStream.writeInt(numberOfImages);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                for (File imageFile : imageFiles) {
                    try {
                        try {
                            transferImage(imageFile);
                            transferredImages++;

                        } catch (Exception e1) {
                            Log.e("TCP", "S: Error:", e1);
                        }
                    } catch (Exception e2) {
                        Log.e("TCP", "S: Error:", e2);
                    }

                    int myProgress = (transferredImages / numberOfImages) * 100;
                    String message = myProgress + "%";
                    builder.setProgress(100, myProgress, false);
                    builder.setContentText(message);
                    nm.notify(1, builder.build());
                }
                try {
                    String toSend = "Stop Transfer\n";
                    mOutputStream.write(toSend.getBytes(), 0, toSend.getBytes().length);
                    mOutputStream.flush();

                    builder.setContentTitle("Finished");
                    builder.setContentText("Image Service has finished backing up your photos.");
                    nm.notify(1, builder.build());

                } catch (Exception e3) {
                    Log.e("TCP", "S: Error:", e3);

                    builder.setContentTitle("Error");
                    builder.setContentText("Image Service could not transfer your photos.");
                    nm.notify(1, builder.build());
                }
            }
        });

        thread.start();
    }

    private boolean transferImage(File ImageFile) {
        try {
            byte[] imageName = ImageFile.getName().getBytes();
            byte[] imageData = Files.readAllBytes(ImageFile.toPath());

            mOutputStream.writeInt(imageName.length);
            mOutputStream.write(imageName);
            mOutputStream.writeInt(imageData.length);
            mOutputStream.write(imageData, 0, imageData.length);


            mOutputStream.flush();
        } catch (Exception e) {
            Log.e("TCP", "Error transferring image" + ImageFile.getName() + ".", e);
            return false;
        }
        return true;
    }

    private void getConnection() {
        try {
            InetAddress serverAddress = InetAddress.getByName(mIPAddress);
            Socket socket = new Socket(serverAddress, mPort);
            try {
                // Supposed to send the message to the ImageServer...
                mOutputStream = new DataOutputStream(socket.getOutputStream());

            } catch (Exception e) {
                Log.e("TCP", "S: Error", e);
            }
        } catch (Exception e) {
            Log.e("TCP", "C: Error", e);
        }
    }
}
