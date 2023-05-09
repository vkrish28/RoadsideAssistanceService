using RoadsideAssistanceBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadsideAssistanceBusiness.Interfaces
{
    public interface IRoadsideAssistanceService
    {
        /**
        * This method is used to update the location of the roadside assistance service provider.
        *
        * @param assistant represents the roadside assistance service provider
        * @param assistantLocation represents the location of the roadside assistant
        */
        public Task UpdateAssistantLocation(Assistant assistant, Geolocation assistantLocation);

        /**
        * This method returns a collection of roadside assistants ordered by their distance from the input geo location.
        *
        * @param geolocation - geolocation from which to search for assistants
        * @param limit - the number of assistants to return
        * @return a sorted collection of assistants ordered ascending by distance from geoLocation
        */
        public Task<List<Assistant>> FindNearestAssistants(Geolocation geolocation, int limit);

        /**
        * This method reserves an assistant for a Geico customer that is stranded on the roadside due to a disabled vehicle.
        *
        * @param customer - Represents a Geico customer
        * @param customerLocation - Location of the customer
        * @return The Assistant that is on their way to help
        */
        public Task<Assistant?> ReserveAssistant(Customer customer, Geolocation customerLocation);

        /**
        * This method releases an assistant either after they have completed work, or the customer no longer needs help.
        *
        * @param customer - Represents a Geico customer
        * @param assistant - An assistant that was previously reserved by the customer
        */
        public Task ReleaseAssistant(Customer customer, Assistant assistant);
    }
}
