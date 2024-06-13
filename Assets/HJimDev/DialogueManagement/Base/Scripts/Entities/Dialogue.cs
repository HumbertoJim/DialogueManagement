using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueManagement
{
    namespace Base
    {
        namespace Entities
        {
            public class Dialogue
            {
                public string ID { get; }

                public int CurrentLine { private set; get; }

                public string CurrentDialoguer { private set; get; }

                readonly string[] content;

                public int Length { get { return content == null ? 0 : content.Length; } }

                public Dialogue(string dialogueID, string[] content, int startAt = 0)
                {
                    ID = dialogueID;
                    CurrentLine = startAt;
                    CurrentDialoguer = "";
                    this.content = content;
                }

                public Line Next(bool ignoreDialoguer = false)
                {
                    if (IsEnded())
                    {
                        return new Line();
                    }
                    else
                    {
                        string line = content[CurrentLine].Trim();
                        if (!ignoreDialoguer)
                        {
                            if (line != "" && line[^1] == ':')
                            {
                                CurrentDialoguer = line[..^1].Trim();
                            }
                        }
                        CurrentLine++;
                        return new Line(line);
                    }
                }

                public bool IsEnded()
                {
                    if (content != null && CurrentLine < content.Length)
                    {
                        return false;
                    }
                    return true;
                }

                public class Line
                {
                    public bool NotNull { get; }
                    public string Text { get; }

                    public Line(string text = null)
                    {
                        if (text == null)
                        {
                            NotNull = false;
                        }
                        else
                        {
                            NotNull = true;
                            Text = text.Trim();
                        }
                    }
                }
            }
        }
    }
}