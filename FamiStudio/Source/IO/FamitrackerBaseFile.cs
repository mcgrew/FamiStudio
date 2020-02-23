﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamiStudio
{
    public class FamitrackerFileBase
    {
        protected static readonly int[] VibratoSpeedImportLookup = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 11, 12 };
        protected static readonly int[] VibratoSpeedExportLookup = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15 };

        protected const byte SndChip_NONE = 0;
        protected const byte SndChip_VRC6 = 1;  // Konami VRCVI
        protected const byte SndChip_VRC7 = 2;  // Konami VRCVII
        protected const byte SndChip_FDS = 4;  // Famicom Disk Sound
        protected const byte SndChip_MMC5 = 8;  // Nintendo MMC5
        protected const byte SndChip_N163 = 16; // Namco N-106
        protected const byte SndChip_S5B = 32; // Sunsoft 5B

        protected const byte Effect_None = 0;
        protected const byte Effect_Speed = 1;
        protected const byte Effect_Jump = 2;
        protected const byte Effect_Skip = 3;
        protected const byte Effect_Halt = 4;
        protected const byte Effect_Volume = 5;
        protected const byte Effect_Portamento = 6;
        protected const byte Effect_Sweepup = 8;
        protected const byte Effect_Sweepdown = 9;
        protected const byte Effect_Arpeggio = 10;
        protected const byte Effect_Vibrato = 11;
        protected const byte Effect_Tremolo = 12;
        protected const byte Effect_Pitch = 13;
        protected const byte Effect_Delay = 14;
        protected const byte Effect_Dac = 15;
        protected const byte Effect_PortaUp = 16;
        protected const byte Effect_PortaDown = 17;
        protected const byte Effect_DutyCycle = 18;
        protected const byte Effect_SampleOffset = 19;
        protected const byte Effect_SlideUp = 20;
        protected const byte Effect_SlideDown = 21;
        protected const byte Effect_VolumeSlide = 22;
        protected const byte Effect_NoteCut = 23;
        protected const byte Effect_Retrigger = 24;
        protected const byte Effect_FdsModDepth = 26;
        protected const byte Effect_FdsModSpeedHi = 27;
        protected const byte Effect_FdsModSpeedLo = 28;
        protected const byte Effect_DpcmPitch = 29;
        protected const byte Effect_SunsoftEnvLo = 30;
        protected const byte Effect_SunsoftEnvHi = 31;
        protected const byte Effect_SunsoftEnvType = 32;
        protected const byte Effect_Count = 33;

        protected static readonly int[] ChanIdLookup = new[]
        {
            Channel.Square1,        // CHANID_SQUARE1
            Channel.Square2,        // CHANID_SQUARE2
            Channel.Triangle,       // CHANID_TRIANGLE
            Channel.Noise,          // CHANID_NOISE
            Channel.Dpcm,           // CHANID_DPCM
            Channel.Vrc6Square1,    // CHANID_VRC6_PULSE1
            Channel.Vrc6Square2,    // CHANID_VRC6_PULSE2
            Channel.Vrc6Saw,        // CHANID_VRC6_SAWTOOTH
            Channel.Mmc5Square1,    // CHANID_MMC5_SQUARE1
            Channel.Mmc5Square2,    // CHANID_MMC5_SQUARE2
            Channel.Mmc5Dpcm,       // CHANID_MMC5_VOICE
            Channel.NamcoWave1,     // CHANID_N163_CHAN1
            Channel.NamcoWave2,     // CHANID_N163_CHAN2
            Channel.NamcoWave3,     // CHANID_N163_CHAN3
            Channel.NamcoWave4,     // CHANID_N163_CHAN4
            Channel.NamcoWave5,     // CHANID_N163_CHAN5
            Channel.NamcoWave6,     // CHANID_N163_CHAN6
            Channel.NamcoWave7,     // CHANID_N163_CHAN7
            Channel.NamcoWave8,     // CHANID_N163_CHAN8
            Channel.FdsWave,        // CHANID_FDS
            Channel.Vrc7Fm1,        // CHANID_VRC7_CH1
            Channel.Vrc7Fm2,        // CHANID_VRC7_CH2
            Channel.Vrc7Fm3,        // CHANID_VRC7_CH3
            Channel.Vrc7Fm4,        // CHANID_VRC7_CH4
            Channel.Vrc7Fm5,        // CHANID_VRC7_CH5
            Channel.Vrc7Fm6,        // CHANID_VRC7_CH6
            Channel.SunsoftSquare1, // CHANID_S5B_CH1
            Channel.SunsoftSquare2, // CHANID_S5B_CH2
            Channel.SunsoftSquare3  // CHANID_S5B_CH3
        };

        protected static int[] InstrumentTypeLookup =
        {
            Project.ExpansionCount,  // INST_NONE: Should never happen.
            Project.ExpansionNone,   // INST_2A03
            Project.ExpansionVrc6,   // INST_VRC6
            Project.ExpansionVrc7,   // INST_VRC7
            Project.ExpansionFds,    // INST_FDS
            Project.ExpansionNamco,  // INST_N163
            Project.ExpansionSunsoft // INST_S5B
        };

        protected static int[] EnvelopeTypeLookup =
        {
            Envelope.Volume,   // SEQ_VOLUME
            Envelope.Arpeggio, // SEQ_ARPEGGIO
            Envelope.Pitch,    // SEQ_PITCH
            Envelope.Max,      // SEQ_HIPITCH
            Envelope.DutyCycle // SEQ_DUTYCYCLE
        };

        protected struct RowFxData
        {
            public byte fx;
            public byte param;
        }

        protected int ConvertExpansionAudio(int exp)
        {
            switch (exp)
            {
                case SndChip_NONE : return Project.ExpansionNone;
                case SndChip_VRC6 : return Project.ExpansionVrc6;
                case SndChip_VRC7 : return Project.ExpansionVrc7;
                case SndChip_FDS  : return Project.ExpansionFds;
                case SndChip_MMC5 : return Project.ExpansionMmc5;
                case SndChip_N163 : return Project.ExpansionNamco;
                case SndChip_S5B  : return Project.ExpansionSunsoft;
            }

            return -1; // We dont support exotic combinations.
        }

        protected void ApplySimpleEffects(RowFxData fx, Pattern pattern, int n, Dictionary<Pattern, byte> patternLengths)
        {
            switch (fx.fx)
            {
                case Effect_Jump:
                    pattern.Song.SetLoopPoint(fx.param);
                    break;
                case Effect_Skip:
                    patternLengths[pattern] = (byte)(n + 1);
                    break;
                case Effect_Speed:
                    if (fx.param <= 0x1f) // We only support speed change for now.
                        pattern.Notes[n].Speed = fx.param;
                    break;
                case Effect_Pitch:
                    pattern.Notes[n].FinePitch = (sbyte)(0x80 - fx.param);
                    break;
                case Effect_Vibrato:
                    pattern.Notes[n].VibratoDepth = (byte)(fx.param & 0x0f);
                    pattern.Notes[n].VibratoSpeed = (byte)VibratoSpeedImportLookup[fx.param >> 4];

                    if (pattern.Notes[n].VibratoDepth == 0 ||
                        pattern.Notes[n].VibratoSpeed == 0)
                    {
                        pattern.Notes[n].Vibrato = 0;
                    }
                    break;
            }
        }
        
        private int FindPrevNoteForPortamento(Channel channel, int patternIdx, int noteIdx, Dictionary<Pattern, RowFxData[,]> patternFxData)
        {
            for (int n = noteIdx - 1; n >= 0; n--)
            {
                var tmpNote = channel.PatternInstances[patternIdx].Notes[n];
                if (tmpNote.IsMusical || tmpNote.IsStop)
                    return tmpNote.Value;
            }

            for (var p = patternIdx - 1; p >= 0; p--)
            {
                var pattern = channel.PatternInstances[p];
                if (pattern != null)
                {
                    for (int n = channel.Song.GetPatternLength(p) - 1; n >= 0; n--)
                    {
                        var tmpNote = pattern.Notes[n];
                        if (tmpNote.IsMusical || tmpNote.IsStop)
                            return tmpNote.Value;
                    }
                }
            }

            return Note.NoteInvalid;
        }

        private bool FindNextNoteForSlide(Channel channel, int patternIdx, int noteIdx, out int nextPatternIdx, out int nextNoteIdx, Dictionary<Pattern, RowFxData[,]> patternFxData)
        {
            nextPatternIdx = -1;
            nextNoteIdx = -1;

            var pattern = channel.PatternInstances[patternIdx];
            var patternLen = channel.Song.GetPatternLength(patternIdx);
            var fxData = patternFxData[pattern];

            for (int n = noteIdx + 1; n < patternLen; n++)
            {
                var fxChanged = false;
                for (int i = 0; i < fxData.GetLength(1); i++)
                {
                    var fx = fxData[n, i];
                    if (fx.fx == Effect_PortaUp || fx.fx == Effect_PortaDown || fx.fx == Effect_Portamento || fx.fx == Effect_SlideUp || fx.fx == Effect_SlideDown)
                    {
                        fxChanged = true;
                        break;
                    }
                }

                var tmpNote = pattern.Notes[n];
                if (tmpNote.IsMusical || tmpNote.IsStop || fxChanged)
                {
                    nextPatternIdx = patternIdx;
                    nextNoteIdx = n;
                    return true;
                }
            }

            for (int p = patternIdx + 1; p < channel.Song.Length; p++)
            {
                pattern = channel.PatternInstances[p];
                patternLen = channel.Song.GetPatternLength(p);
                fxData = patternFxData[pattern];

                for (int n = 0; n < patternLen; n++)
                {
                    var fxChanged = false;
                    for (int i = 0; i < fxData.GetLength(1); i++)
                    {
                        var fx = fxData[n, i];
                        if (fx.fx == Effect_PortaUp || fx.fx == Effect_PortaDown || fx.fx == Effect_Portamento || fx.fx == Effect_SlideUp || fx.fx == Effect_SlideDown)
                        {
                            fxChanged = true;
                            break;
                        }
                    }

                    var tmpNote = channel.PatternInstances[p].Notes[n];
                    if (tmpNote.IsMusical || tmpNote.IsStop || fxChanged)
                    {
                        nextPatternIdx = p;
                        nextNoteIdx = n;
                        return true;
                    }
                }
            }

            return false;
        }

        private int FindBestMatchingNote(ushort[] noteTable, int pitch, int sign)
        {
            var bestIdx  = -1;
            var bestDiff = 99999;

            for (int i = 1; i < noteTable.Length; i++)
            {
                var diff = (pitch - noteTable[i]) * sign;
                if (diff >= 0 && diff < bestDiff)
                {
                    bestIdx = i;
                    bestDiff = diff;
                }
            }

            return bestIdx;
        }

        private void CreateSlideNotes(Song s, Dictionary<Pattern, RowFxData[,]> patternFxData)
        {
            // Convert slide notes + portamento to our format.
            foreach (var c in s.Channels)
            {
                if (!c.SupportsSlideNotes)
                    continue;

                var lastNoteInstrument = (Instrument)null;
                var lastNoteValue = (byte)Note.NoteInvalid;
                var portamentoSpeed = 0;

                for (int p = 0; p < s.Length; p++)
                {
                    var pattern = c.PatternInstances[p];

                    if (pattern == null)
                        continue;

                    var fxData = patternFxData[pattern];

                    for (int n = 0; n < s.GetPatternLength(p); n++)
                    {
                        var note = pattern.Notes[n];
                        var slideSpeed = 0;
                        var slideTarget = 0;

                        for (int i = 0; i < fxData.GetLength(1); i++)
                        {
                            var fx = fxData[n, i];

                            if (fx.param != 0)
                            {
                                // When the effect it turned on, we need to add a note.
                                if ((fx.fx == Effect_PortaUp || fx.fx == Effect_PortaDown || fx.fx == Effect_SlideUp || fx.fx == Effect_SlideDown) && lastNoteValue >= Note.MusicalNoteMin && lastNoteValue <= Note.MusicalNoteMax && !note.IsValid)
                                {
                                    pattern.Notes[n].Value = lastNoteValue;
                                    pattern.Notes[n].Instrument = lastNoteInstrument;
                                    pattern.Notes[n].HasAttack = false;
                                    note = pattern.Notes[n];
                                }

                                if (fx.fx == Effect_PortaUp) slideSpeed = -fx.param;
                                if (fx.fx == Effect_PortaDown) slideSpeed =  fx.param;
                                if (fx.fx == Effect_Portamento)
                                {
                                    portamentoSpeed = fx.param;
                                }
                                if (fx.fx == Effect_SlideUp)
                                {
                                    slideTarget = note.Value + (fx.param & 0xf);
                                    slideSpeed = -((fx.param >> 4) * 2 + 1);
                                }
                                if (fx.fx == Effect_SlideDown)
                                {
                                    slideTarget = note.Value - (fx.param & 0xf);
                                    slideSpeed = ((fx.param >> 4) * 2 + 1);
                                }
                            }
                            else if (fx.fx == Effect_Portamento)
                            {
                                portamentoSpeed = 0;
                            }
                        }

                        // Create a slide note.
                        if (!note.IsSlideNote)
                        {
                            if (note.IsMusical)
                            {
                                var noteTable = NesApu.GetNoteTableForChannelType(c.Type, false);
                                var pitchLimit = NesApu.GetPitchLimitForChannelType(c.Type);

                                // If we have a new note with auto-portamento enabled, we need to
                                // swap the notes since our slide notes work backward compared to 
                                // FamiTracker.
                                if (portamentoSpeed != 0)
                                {
                                    if (lastNoteValue >= Note.MusicalNoteMin && lastNoteValue <= Note.MusicalNoteMax)
                                    {
                                        pattern.Notes[n].SlideNoteTarget = pattern.Notes[n].Value;
                                        pattern.Notes[n].Value = lastNoteValue;
                                    }
                                }
                                else if (slideTarget != 0)
                                {
                                    var numFrames = Math.Abs((noteTable[note.Value] - noteTable[slideTarget]) / (slideSpeed * s.Speed));
                                    pattern.Notes[n].SlideNoteTarget = (byte)slideTarget;

                                    var nn = n + numFrames;
                                    var np = p;
                                    while (nn >= s.GetPatternLength(np))
                                    {
                                        nn -= s.GetPatternLength(np);
                                        np++;
                                    }
                                    if (np >= s.Length)
                                    {
                                        np = s.Length;
                                        nn = 0;
                                    }

                                    // Still to see if there is a note between the current one and the 
                                    // next note, this could append if you add a note before the slide 
                                    // is supposed to finish.
                                    if (FindNextNoteForSlide(c, p, n, out var np2, out var nn2, patternFxData))
                                    {
                                        if (np2 < np)
                                        {
                                            np = np2;
                                            nn = nn2;
                                        }
                                        else if (np2 == np)
                                        {
                                            nn = Math.Min(nn, nn2);
                                        }
                                    }

                                    // Add an extra note with no attack to stop the slide.
                                    var nextPattern = c.PatternInstances[np];
                                    if (!nextPattern.Notes[nn].IsValid)
                                    {
                                        nextPattern.Notes[nn].Instrument = note.Instrument;
                                        nextPattern.Notes[nn].Value = (byte)slideTarget;
                                        nextPattern.Notes[nn].HasAttack = false;
                                    }
                                }
                                // Find the next note that would stop the slide or change the FX settings.
                                else if (slideSpeed != 0 && FindNextNoteForSlide(c, p, n, out var np, out var nn, patternFxData))
                                {
                                    // Compute the pitch delta and find the closest target note.
                                    var numFrames = (s.GetPatternStartNote(np, nn) - s.GetPatternStartNote(p, n)) * s.Speed;

                                    // TODO: PAL.
                                    var newNotePitch = Utils.Clamp(noteTable[note.Value] + numFrames * slideSpeed, 0, pitchLimit);
                                    var newNote = FindBestMatchingNote(noteTable, newNotePitch, Math.Sign(slideSpeed));

                                    pattern.Notes[n].SlideNoteTarget = (byte)newNote;

                                    // If the FX was turned off, we need to add an extra note.
                                    var nextPattern = c.PatternInstances[np];
                                    if (!nextPattern.Notes[nn].IsMusical &&
                                        !nextPattern.Notes[nn].IsStop)
                                    {
                                        nextPattern.Notes[nn].Instrument = note.Instrument;
                                        nextPattern.Notes[nn].Value = (byte)newNote;
                                        nextPattern.Notes[nn].HasAttack = false;
                                    }
                                }
                            }
                        }

                        if (note.IsMusical || note.IsStop)
                        {
                            lastNoteValue = note.Value;
                            lastNoteInstrument = note.Instrument;
                        }
                    }
                }
            }
        }

        protected bool FinishImport(Project project, Dictionary<Pattern, byte> patternLengths, Dictionary<Pattern, RowFxData[,]> patternFxData)
        {
            foreach (var s in project.Songs)
            {
                foreach (var c in s.Channels)
                {
                    c.ColorizePatterns();

                    for (int p = 0; p < s.Length; p++)
                    {
                        var pattern = c.PatternInstances[p];
                        if (pattern != null && patternLengths.TryGetValue(pattern, out var instLength))
                            s.SetPatternLength(p, instLength);
                    }
                }

                s.RemoveEmptyPatterns();
                s.SetSensibleBarLength();
                s.UpdatePatternStartNotes();
                s.UpdatePatternsMaxInstanceLength();

                CreateSlideNotes(s, patternFxData);
            }

            project.UpdateAllLastValidNotesAndVolume();

            return true;
        }
    };
}