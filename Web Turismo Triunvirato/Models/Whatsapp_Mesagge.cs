namespace Web_Turismo_Triunvirato.Models
{
    public class WhatsappMessage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message_Template { get; set; }
        public bool Is_Active { get; set; }



        public static string RenderWts<T>(string messageTemplate, T dataObject)
        {
            string renderedMessage = messageTemplate;
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var placeholder = $"@{prop.Name}"; // Asegúrate que coincida con el placeholder
                var value = prop.GetValue(dataObject)?.ToString();

                if (value != null)
                {
                    renderedMessage = renderedMessage.Replace(placeholder, value);
                }
            }

            return renderedMessage;
        }


    }
}
