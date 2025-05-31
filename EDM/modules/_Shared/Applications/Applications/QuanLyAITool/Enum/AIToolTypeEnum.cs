using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Applications.Enums
{
    public enum AIToolTypeEnum
    {
        [Display(Name = "OpenAI - ChatGPT")]
        OpenAI_ChatGPT,

        [Display(Name = "Google - Gemini")]
        Google_Gemini,

        [Display(Name = "Azure - OpenAI")]
        Azure_OpenAI,

        [Display(Name = "Anthropic - Claude")]
        Anthropic_Claude,

        [Display(Name = "Cohere")]
        Cohere,

        [Display(Name = "Mistral")]
        Mistral
    }
}