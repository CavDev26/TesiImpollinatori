using UnityEngine;

// Base comune a PiantaData e ImpollinatoreData: stesso identico schema minimale di ZollettaData
// (nome + icona), cosi' il pannello dettagli puo' mostrare entrambe le liste con lo stesso codice.
public abstract class VoceGlossario : ScriptableObject
{
    public string nome;
    public Sprite icona;
}
