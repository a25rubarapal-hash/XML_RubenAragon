using UnityEngine;
using System.IO;
using TMPro;
using System.Collections;

[System.Serializable]
public class DatosPartida
{
    public int totalRebotes;
}

public class JSON : MonoBehaviour
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
        rutaArchivo = Path.Combine(rutaCarpeta, "datos.json");

        if (textoNotificacion != null) textoNotificacion.text = "";

        if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

        DatosPartida datosIniciales = new DatosPartida { totalRebotes = 0 };
        string jsonInicial = JsonUtility.ToJson(datosIniciales, true);
        File.WriteAllText(rutaArchivo, jsonInicial);

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

        DatosPartida datos = new DatosPartida();
        datos.totalRebotes = rebotesActuales;

        string formatoJson = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaArchivo, formatoJson);

        StartCoroutine(MostrarNotificacion("Datos JSON guardados con exito"));
    }

    public void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            string jsonLeido = File.ReadAllText(rutaArchivo);
            DatosPartida datos = JsonUtility.FromJson<DatosPartida>(jsonLeido);

            rebotesActuales = datos.totalRebotes;

            XML scriptXml = GetComponent<XML>();
            if (scriptXml != null) scriptXml.rebotesActuales = datos.totalRebotes;

            ActualizarPantalla();
            StartCoroutine(MostrarNotificacion("Datos JSON cargados con exito"));
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