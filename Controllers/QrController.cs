using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.IO;

public class QRController : Controller
{
    // GET /QR/Generar?token=xxxxx
    public IActionResult Generar(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return BadRequest();

        // Construir URL absoluta hacia la acción Marcar en AsistenciaController
        var url = Url.Action("Marcar", "Asistencia", new { token = token }, Request.Scheme);

        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using (var qrCode = new QRCode(qrData))
            {
                using (var bitmap = qrCode.GetGraphic(20))
                {
                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return File(ms.ToArray(), "image/png");
                    }
                }
            }
        }
    }
}
