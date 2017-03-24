﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Larch.Host {
    public class Filter {
        private readonly Regex _pattern;
        private readonly string _text;

        public Filter(string pattern, CampareType type = CampareType.WildCard, CompareMode mode = CompareMode.CaseIgnore) {
            if (pattern == null) {
                _pattern = null;
                _text = null;
                return;
            }

            var regexOptions = RegexOptions.None;
            if (mode == CompareMode.CaseIgnore) {
                regexOptions = RegexOptions.IgnoreCase;
            }

            switch (type) {
                default:
                case CampareType.WildCard:
                    if (pattern.Contains("*") || pattern.Contains("?")) {
                        _pattern = new Regex($"^{Regex.Escape(pattern).Replace(@"\?", ".").Replace(@"\*", ".*")}$", regexOptions);
                    } else {
                        _text = pattern;
                    }
                    break;
                case CampareType.Regex:
                    _pattern = new Regex(pattern, regexOptions);
                    break;
            }
        }

        public bool IsMatch(string value) {
            if (value == null) value = string.Empty;

            if (_text != null) {
                return value == _text;
            }

            if (_pattern != null) {
                return _pattern.IsMatch(value);
            }

            // if we have no filter pattern we do not filter
            return true;
        }


        public Match<T> GetMatch<T>(string value, T model) {
            if (value == null) value = string.Empty;

            if (_text != null) {
                return new Match<T>() {
                    IsSuccess = _text == value,
                    Matches = GetMatchs(value),
                    Model = model,
                    Value = value
                };
            }

            if (_pattern != null) {
                return new Match<T>() {
                    IsSuccess = _pattern.IsMatch(value),
                    Matches = GetMatchs(value),
                    Model = model,
                    Value = value
                };
            }

            // if we have no filter pattern we do not filter
            return new Match<T>() {
                IsSuccess = true,
                Matches = GetMatchs(value),
                Model = model,
                Value = value
            };
        }

        private IEnumerable<Group> GetMatchs(string value) {
            if (_text != null) {
                yield return new Group() {
                    Start = 0,
                    Stop = value.Length,
                    IsMatch = _text == value
                };

                yield break;
            }

            if (_pattern != null) {
                var matchs = _pattern.Matches(value);
                for (var i = 0; i < matchs.Count; i++) {
                    for (var j = 0; j < matchs[i].Groups.Count; j++) {
                        yield return new Group() {
                            Start = matchs[i].Groups[j].Index,
                            Stop = matchs[i].Groups[j].Index + matchs[i].Groups[j].Length,
                            IsMatch = matchs[i].Groups[j].Success
                        };
                    }
                }

                yield break;
            }

            yield return new Group() {
                Start = 0,
                Stop = value.Length,
                IsMatch = true
            };
        }
    }


    public class Match<T> {
        public IEnumerable<Group> Matches;
        public T Model;
        public bool IsSuccess;
        public string Value;
    }


    public class Group {
        public int Start { get; set; }
        public int Stop { get; set; }
        public bool IsMatch { get; set; }
    }


    public enum CompareMode {
        CaseIgnore = 0,
        ExactMatch = 2,
    }


    public enum CampareType {
        WildCard = 0,
        Regex = 1
    }
}