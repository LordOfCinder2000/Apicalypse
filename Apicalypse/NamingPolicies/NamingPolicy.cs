namespace Apicalypse.NamingPolicies
{
    public abstract class NamingPolicy
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NamingPolicy"/>.
        /// </summary>
        public NamingPolicy() { }

        /// <summary>
        /// Returns the naming policy for camel-casing.
        /// </summary>
        public static NamingPolicy CamelCase { get; } = new CamelCaseNamingPolicy();

        /// <summary>
        /// Returns the naming policy for snake-casing.
        /// </summary>
        public static NamingPolicy SnakeCase { get; } = new SnakeCaseNamingPolicy();

        /// <summary>
        /// Returns the naming policy for PascalCase.
        /// </summary>
        public static NamingPolicy PascalCase { get; } = new PascalCaseNamingPolicy();

        /// <summary>
        /// When overridden in a derived class, converts the specified name according to the policy.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted name.</returns>
        public abstract string ConvertName(string name);
    }
}
