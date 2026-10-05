using UnityEngine;
using System.IO;
using TMPro;
using System.Collections;
using System.Xml;

public class XML : MonoBehaviour
{
    public int rebotesActuales = 0;
    public TMP_Text textoContador;
    public TMP_Text textoNotificacion;

    private string rutaCarpeta;
    private string rutaArchivo;
    private bool puedeRebotar = true;

    void Start()
    {
        rutaCarpeta = Path.Combine(Application.persistentDataPath, "CarpetaRebotes");
        rutaArchivo = Path.Combine(rutaCarpeta, "datos.xml");

        if (textoNotificacion != null) textoNotificacion.text = "";

        if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("DatosPartidaXML");
        xmlDoc.AppendChild(root);

        XmlElement elementoRebotes = xmlDoc.CreateElement("totalRebotes");
        elementoRebotes.InnerText = "0";
        root.AppendChild(elementoRebotes);
        xmlDoc.Save(rutaArchivo);

        rebotesActuales = 0;
        ActualizarPantalla();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo") && puedeRebotar)
        {
            rebotesActuales++;
            ActualizarPantalla();
            StartCoroutine(CooldownRebote());
        }
    }

    private IEnumerator CooldownRebote()
    {
        puedeRebotar = false;
        yield return new WaitForSeconds(0.1f);
        puedeRebotar = true;
    }

    private void ActualizarPantalla()
    {
        if (textoContador != null) textoContador.text = rebotesActuales.ToString();
    }

    public void GuardarDatos()
    {
        if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("DatosPartidaXML");
        xmlDoc.AppendChild(root);

        XmlElement elementoRebotes = xmlDoc.CreateElement("totalRebotes");
        elementoRebotes.InnerText = rebotesActuales.ToString();
        root.AppendChild(elementoRebotes);

        xmlDoc.Save(rutaArchivo);
        StartCoroutine(MostrarNotificacion("Datos XML guardados con exito"));
    }

    public void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(rutaArchivo);
            XmlNode nodoRebotes = xmlDoc.SelectSingleNode("DatosPartidaXML/totalRebotes");

            if (nodoRebotes != null && int.TryParse(nodoRebotes.InnerText, out int resultado))
            {
                rebotesActuales = resultado;

                JSON scriptJson = GetComponent<JSON>();
                if (scriptJson != null) scriptJson.rebotesActuales = resultado;

                ActualizarPantalla();
                StartCoroutine(MostrarNotificacion("Datos XML cargados con exito"));
            }
        }
    }

    private IEnumerator MostrarNotificacion(string mensaje)
    {
        if (textoNotificacion != null)
        {
            textoNotificacion.text = mensaje;
            yield return new WaitForSeconds(1f);
            textoNotificacion.text = "";
        }
    }
}