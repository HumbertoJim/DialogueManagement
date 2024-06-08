using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace VRMAgentBased
    {
        public class DialoguerCluster: Core.DialoguerCluster
        {
            // FALTA AQUI LA REFERENCIA A CHARACTER_DATA, PARA ALMACENAR COSAS COMO LA RELACION ENTRE JUGADOR Y PERSONAJES

            public new Dialoguer MainCharacter { get { return (Dialoguer)base.MainCharacter; } }

            public new Dialoguer GetDialoguer(string code)
            {
                return (Dialoguer)base.GetDialoguer(code);
            }

            public void ImproveRelationship(string code, int value)
            {

            }

            public void DecreaseRelationship(string code, int value)
            {

            }

            public void RegisterInteraction(string code)
            {

            }
        }
    }
}