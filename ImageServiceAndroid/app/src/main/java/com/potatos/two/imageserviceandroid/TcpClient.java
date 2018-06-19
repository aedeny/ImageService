package com.potatos.two.imageserviceandroid;

import android.app.NotificationManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Environment;
import android.support.v4.app.NotificationCompat;
import android.util.Log;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;

class TcpClient {
    private OutputStream outputStream;

    public void connect(final NotificationManager notificationManager, final NotificationCompat
            .Builder builder) {
        Thread thread = new Thread(new Runnable() {
            @Override
            public void run() {
                getConnection();
                File dcim = new File(Environment.getExternalStoragePublicDirectory(Environment
                        .DIRECTORY_DCIM), "Camera");

                File[] files = dcim.listFiles();
                double numberOfPictures = files.length;
                double count = 0;

                for (File file : files) {
                    try {
                        FileInputStream fis = new FileInputStream(file);
                        Bitmap bm = BitmapFactory.decodeStream(fis);
                        byte[] imgbyte = getBytesFromBitmap(bm);
                        try {
                            int imageLength = imgbyte.length;

                            // Sends the size of the array bytes.
                            String picSizeString = imageLength + "";
                            outputStream.write(picSizeString.getBytes(), 0, picSizeString
                                    .getBytes().length);

                            //sends the name of file.
                            String fileNameString = file.getName();
                            outputStream.write(fileNameString.getBytes(), 0, fileNameString
                                    .getBytes().length);

                            //sends the array bytes.
                            outputStream.write(imgbyte, 0, imgbyte.length);
                            outputStream.flush();

                        } catch (Exception e1) {
                            Log.e("TCP", "S: Error:", e1);
                        }
                    } catch (Exception e2) {
                        Log.e("TCP", "S: Error:", e2);
                    }

                    count++;
                    int myProgress = (int) ((count / numberOfPictures) * 100);
                    String message = myProgress + "%";
                    builder.setProgress(100, myProgress, false);
                    builder.setContentText(message);
                    notificationManager.notify(1, builder.build());

                }
                try {
                    String toSend = "Stop Transfer\n";
                    outputStream.write(toSend.getBytes(), 0, toSend.getBytes().length);
                    outputStream.flush();

                    builder.setContentTitle("Finish transfer");
                    builder.setContentText("Image Service finish backing up your photos");
                    notificationManager.notify(1, builder.build());

                } catch (Exception e3) {
                    Log.e("TCP", "S: Error:", e3);

                    builder.setContentTitle("Error");
                    builder.setContentText("Image Service could not transfer your photos");
                    notificationManager.notify(1, builder.build());
                }
            }
        });

        thread.start();
    }

    private void getConnection() {
        try {
            // Computer's IP address
            InetAddress serverAddr = InetAddress.getByName("10.0.2.2");

            // Creates a socket to make the connection with the server
            Socket socket = new Socket(serverAddr, 3748);
            try {
                // Supposed to send the message to the ImageServer...
                outputStream = socket.getOutputStream();
                // TODO Remove commented code without fear. Maximum effort.
//                FileInputStream fileInputStream = new FileInputStream(pic);
//                Bitmap bitmap = BitmapFactory.decodeStream(fileInputStream);
//                byte[] imgbyte = getBytesFromBitmap(bitmap);
                // ...
//                outputStream.write(imgbyte);
//                outputStream.flush();
            } catch (Exception e) {
                Log.e("TCP", "S: Error", e);
            }
        } catch (Exception e) {
            Log.e("TCP", "C: Error", e);
        }
    }

    private byte[] getBytesFromBitmap(Bitmap bitmap) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.PNG, 70, stream);

        return stream.toByteArray();
    }
}
