using System;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Utils;
using QRCoder;
using QRCoder.Unity;

[RequireComponent(typeof(Image))]
public class WalletConnectQRImage : BindableMonoBehavior
{
    /// <summary>
    /// The WalletConnect instance we'll work with to generate the QR code.
    /// </summary>
    public WalletConnect walletConnect;
    
    /// <summary>
    /// The image component we'll place the QR code texture into.
    /// </summary>
    [BindComponent]
    private Image _image;
    
    /// <summary>
    /// Flag to avoid adding additional OnConnectionStarted event handlers every time we disconnect & reconnect to the wallet (i.e. Disable/Enable this component). 
    /// </summary>
    private bool registeredOnConnectionStartEvent = false;
    
    /// <summary>
    /// Unity OnEnable hook.
    /// </summary>
    void OnEnable()
    {
        if (walletConnect == null)
        {
            Debug.LogError("WalletConnectQRImage: No WalletConnect object given, QRImage will be disabled");
            enabled = false;
            return;
        }
        
        // Only register the WalletConnectOnConnectionStarted handler once
        if (!registeredOnConnectionStartEvent)
        {
            registeredOnConnectionStartEvent = true;
            walletConnect.ConnectionStarted += WalletConnectOnConnectionStarted;
        }
    }
    
    /// <summary>
    /// Event handler method to connect to a WalletConnect session and generate a QR code.
    /// </summary>
    /// <param name="sender">Source of event</param>
    /// <param name="e">Event arguments</param>
    private void WalletConnectOnConnectionStarted(object sender, EventArgs e)
    {
        // Grab the WC URL and generate a QR code for it. Note: The ECCLevel is the "Error Correction Code" level which
        // is basically how much checksum data to add to the code - the more checksum data the more likely the code can
        // be recovered on a slightly dodgy read. We'll go with the UnityWalletConnect default of Q(uality) as it's a
        // good compromise between readability and data storage capacity.
        // See: https://www.qrcode.com/en/about/version.html
        var url = walletConnect.Session.URI;
        Debug.Log("Connecting to: " + url);
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);

        // Copy the URL to the clipboard to allow for manual connection in wallet apps that support it
        GUIUtility.systemCopyBuffer = url;
        
        // Create the QR code as a Texture2D. Note: "pixelsPerModule" means the size of each black-or-white block in the
        // QR code image. For example, a size of 2 will give us a 138x138 image (too small!), while 20 will give us a
        // 1380x1380 image (too big!). Here we'll use a value of 10 which gives us a 690x690 pixel image.
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(pixelsPerModule:10);

        // Change the filtering mode to point (i.e. nearest) rather than the default of linear - we want sharp edges on
        // the blocks, not blurry interpolated edges!
        qrCodeAsTexture2D.filterMode = FilterMode.Point;

        // Convert the texture into a sprite and assign it to our QR code image
        var qrCodeSprite = Sprite.Create(qrCodeAsTexture2D, new Rect(0, 0, qrCodeAsTexture2D.width, qrCodeAsTexture2D.height),
            new Vector2(0.5f, 0.5f), 100f);
        _image.sprite = qrCodeSprite;
    }
}