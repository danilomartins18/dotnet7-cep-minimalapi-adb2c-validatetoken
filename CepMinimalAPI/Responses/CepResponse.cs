namespace CepMinimalAPI.Responses
{
	public class CepResponse : CepBasicResponse
	{
		public string Logradouro { get; set; } = string.Empty;
		public string Complemento { get; set; } = string.Empty;
		public string Bairro { get; set; } = string.Empty;
		public string Ibge { get; set; } = string.Empty;
		public string Gia { get; set; } = string.Empty;
		public string Ddd { get; set; } = string.Empty;
		public string Siafi { get; set; } = string.Empty;
	}

}
