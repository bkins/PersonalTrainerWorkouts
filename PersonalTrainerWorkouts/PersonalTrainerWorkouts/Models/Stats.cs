namespace PersonalTrainerWorkouts.Models
{
    public class Stats
    {
        /*
         * Stats will hold client's Current (or keep a history -- start and end date of each)
         * statistics, like:
         * * Measurables
         *    * Columns
         *      * int      ClientId (FK)
         *      * string   Variable (ex: waist, upper arms, but also bench press, dead lift, push ups)
         *      * double   Value
         *      * string   Unit of Measurement (inches, pounds, reps)
         *      * DateTime Date taken
         *      * string   Type (Measurement or Max) (consider a Type table and this column would hold the TypeId.)
         *      
         * * Maxes (separate table -- Bench press, dead lift
         *   * Columns
         *     * ClientId (FK)
         *     * Exercise (FK to Exercises???) (ex: Bench press, dead lift)
         *     * Value
         *     * Unit of Measurement (pounds, reps)
         *     * Date taken
         */
        /*
         * Stats:
         *  * Measurements
         *  * Maxes
         */

    }
}