﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Identity.Api.Constants;
using Identity.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Controllers;

[ApiController]
public class IdentityController(IConfiguration configuration) : ControllerBase
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenGenerationRequest request)
    {
        var jwtKey = configuration[ConfigurationKeys.JwtKey]!;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("userid", request.UserId.ToString())
        };

        foreach (var claimPair in request.CustomClaims)
        {
            var jsonElement = (JsonElement)claimPair.Value;
            var valueType = jsonElement.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                _ => ClaimValueTypes.String
            };

            var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
            claims.Add(claim);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = "https://identity.example.com",
            Audience = "https://app.example.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);
        return Ok(jwt);
    }
}
