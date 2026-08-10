namespace BarberFlow.Application.Features.Collaborators.Exceptions
{
    public class CollaboratorNotFoundException : Exception
    {
        public CollaboratorNotFoundException() : base("Colaborador no encontrado.") { }
    }
}
