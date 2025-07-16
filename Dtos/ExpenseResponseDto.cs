namespace hometask6.Dtos;

public record ExpenseResponseDto(
    Guid Id, 
    Guid CategoryId, 
    decimal Price, 
    string? Comment, 
    DateTime DateTime
    );
