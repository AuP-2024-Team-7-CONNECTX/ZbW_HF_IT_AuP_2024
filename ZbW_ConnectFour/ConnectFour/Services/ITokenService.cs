public interface ITokenService
{
	string GenerateVerificationToken();
	bool ValidateToken(string token);
	void InvalidateToken(string token);
}

public class TokenService : ITokenService
{
	private readonly Dictionary<string, string> _tokens = new Dictionary<string, string>();

	public string GenerateVerificationToken()
	{
		var token = Guid.NewGuid().ToString();
		_tokens[token] = "valid";
		return token;
	}

	public bool ValidateToken(string token)
	{
		if (_tokens.ContainsKey(token) && _tokens[token] == "valid")
		{
			return true;
		}
		return false;
	}

	public void InvalidateToken(string token)
	{
		if (_tokens.ContainsKey(token))
		{
			_tokens[token] = "invalid";
		}
	}
}
