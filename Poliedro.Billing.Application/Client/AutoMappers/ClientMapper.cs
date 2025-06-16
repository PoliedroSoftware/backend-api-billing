using AutoMapper;
using Poliedro.Billing.Application.Client.Commands.CreateClient;
using Poliedro.Billing.Application.Client.Commands.UpdateClient;
using Poliedro.Billing.Application.Client.Dtos;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Application.Client.AutoMappers
{
    public class ClientMapper : Profile
    {
        public ClientMapper()
        {
            CreateMap<ClientEntity, ClientDto>().ReverseMap();
            CreateMap<ClientEntity, CreateClientCommand>().ReverseMap();
            CreateMap<ClientEntity, UpdateClientCommand>().ReverseMap();
        }
    }
}
