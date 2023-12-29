using AutoMapper;

namespace BookingWebAPI.Utils
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Maps the list of the objects to an instance of the <see cref="TResult"/> type.
        /// The mapping happens via applying mapping rules subsequently.
        /// Watch out for cases when two different mapping rules assign value to the same field/property of the <see cref="TResult"/> destination type.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="mapper">The <see cref="IMapper"/> mapper instance.</param>
        /// <param name="objects">The list of objects you want to map to a <see cref="TResult"/> instance.</param>
        /// <returns></returns>
        public static TResult Map<TResult>(this IMapper mapper, params object[] objects)
        {
            var res = mapper.Map<TResult>(objects.First());
            return objects.Skip(1).Aggregate(res, (r, obj) => mapper.Map(obj, r));
        }
    }
}
