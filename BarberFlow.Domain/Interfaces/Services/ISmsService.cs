using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberFlow.Domain.Interfaces.Services;
public interface ISmsService
{
    Task SendAsync(string phoneNumber, string message);
}
