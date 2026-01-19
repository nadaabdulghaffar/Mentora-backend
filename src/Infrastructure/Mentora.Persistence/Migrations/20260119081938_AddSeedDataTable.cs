using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mentora.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "career_goal",
                columns: new[] { "career_goal_id", "name" },
                values: new object[,]
                {
                    { 1, "Grow and Advance in My Current Field" },
                    { 2, "Explore a New Career Path" },
                    { 3, "Start My Own Business or Project" },
                    { 4, "Get Guidance on My Career Journey" },
                    { 5, "Prepare for Leadership or Management Roles" },
                    { 6, "Something Else" }
                });

            migrationBuilder.InsertData(
                table: "countries",
                columns: new[] { "country_code", "country_name" },
                values: new object[,]
                {
                    { "AE", "United Arab Emirates" },
                    { "BH", "Bahrain" },
                    { "DJ", "Djibouti" },
                    { "DZ", "Algeria" },
                    { "EG", "Egypt" },
                    { "IQ", "Iraq" },
                    { "JO", "Jordan" },
                    { "KM", "Comoros" },
                    { "KW", "Kuwait" },
                    { "LB", "Lebanon" },
                    { "LY", "Libya" },
                    { "MA", "Morocco" },
                    { "MR", "Mauritania" },
                    { "OM", "Oman" },
                    { "PS", "Palestine" },
                    { "QA", "Qatar" },
                    { "SA", "Saudi Arabia" },
                    { "SD", "Sudan" },
                    { "SO", "Somalia" },
                    { "SY", "Syria" },
                    { "TN", "Tunisia" },
                    { "YE", "Yemen" }
                });

            migrationBuilder.InsertData(
                table: "domains",
                columns: new[] { "domain_id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Building software systems and applications", "Software Engineering" },
                    { 2, "Machine learning and data-driven systems", "AI & Data Science" },
                    { 3, "User-centered and visual design disciplines", "Design" },
                    { 4, "Product strategy, management, and business skills", "Product & Business" }
                });

            migrationBuilder.InsertData(
                table: "learning_style",
                columns: new[] { "learning_style_id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Learns best through visuals, diagrams, and examples", "Visual" },
                    { 2, "Learns best through listening, discussions, and explanations", "Auditory" },
                    { 3, "Prefers text-based materials, notes, and documentation", "Reading/Writing" },
                    { 4, "Learns best through hands-on practice and experimentation", "Kinesthetic" },
                    { 5, "Learns best by building real projects", "Project-Based" },
                    { 6, "Prefers structured guidance and regular mentor feedback", "Guided Mentorship" },
                    { 7, "Prefers independent learning at own pace", "Self-Paced" },
                    { 8, "Learns best through group discussions and collaboration", "Collaborative" }
                });

            migrationBuilder.InsertData(
                table: "subdomain",
                columns: new[] { "subdomain_id", "domain_id", "name" },
                values: new object[,]
                {
                    { 1, 1, "Backend Development" },
                    { 2, 1, "Frontend Development" },
                    { 3, 1, "Full Stack Development" },
                    { 4, 1, "Mobile Development" },
                    { 5, 1, "DevOps & Cloud" },
                    { 6, 1, "System Design" },
                    { 7, 2, "Machine Learning" },
                    { 8, 2, "Deep Learning" },
                    { 9, 2, "Computer Vision" },
                    { 10, 2, "Natural Language Processing" },
                    { 11, 2, "Data Analysis" },
                    { 12, 2, "Data Science" },
                    { 13, 2, "Data Engineering" },
                    { 14, 3, "UI/UX Design" },
                    { 15, 3, "Product Design" },
                    { 16, 3, "Graphic Design" },
                    { 17, 3, "Motion Design" },
                    { 18, 3, "Branding" },
                    { 19, 4, "Product Management" },
                    { 20, 4, "Business Analysis" },
                    { 21, 4, "Project Management" },
                    { 22, 4, "Career Coaching" },
                    { 23, 4, "Entrepreneurship" }
                });

            migrationBuilder.InsertData(
                table: "technologies",
                columns: new[] { "technology_id", "name", "subdomain_id" },
                values: new object[,]
                {
                    { 1, "Node.js", 1 },
                    { 2, ".NET / ASP.NET Core", 1 },
                    { 3, "Spring Boot", 1 },
                    { 4, "Django", 1 },
                    { 5, "Flask", 1 },
                    { 6, "Laravel", 1 },
                    { 7, "Express.js", 1 },
                    { 8, "FastAPI", 1 },
                    { 9, "NestJS", 1 },
                    { 10, "HTML", 2 },
                    { 11, "CSS", 2 },
                    { 12, "JavaScript", 2 },
                    { 13, "TypeScript", 2 },
                    { 14, "React", 2 },
                    { 15, "Angular", 2 },
                    { 16, "Vue.js", 2 },
                    { 17, "Next.js", 2 },
                    { 18, "Nuxt.js", 2 },
                    { 19, "SASS / SCSS", 2 },
                    { 20, "Tailwind CSS", 2 },
                    { 21, "Bootstrap", 2 },
                    { 22, "MERN Stack", 3 },
                    { 23, "MEAN Stack", 3 },
                    { 24, "LAMP Stack", 3 },
                    { 25, "Django + React", 3 },
                    { 26, "Next.js Full Stack", 3 },
                    { 27, "REST APIs", 3 },
                    { 28, "GraphQL", 3 },
                    { 29, "Flutter", 4 },
                    { 30, "React Native", 4 },
                    { 31, "Android (Kotlin)", 4 },
                    { 32, "Android (Java)", 4 },
                    { 33, "iOS (Swift)", 4 },
                    { 34, "Xamarin", 4 },
                    { 35, "Ionic", 4 },
                    { 36, "Docker", 5 },
                    { 37, "Kubernetes", 5 },
                    { 38, "AWS", 5 },
                    { 39, "Azure", 5 },
                    { 40, "Google Cloud Platform", 5 },
                    { 41, "CI/CD Pipelines", 5 },
                    { 42, "Jenkins", 5 },
                    { 43, "GitHub Actions", 5 },
                    { 44, "GitLab CI", 5 },
                    { 45, "Terraform", 5 },
                    { 46, "Ansible", 5 },
                    { 47, "Nginx", 5 },
                    { 48, "High Level Design (HLD)", 6 },
                    { 49, "Low Level Design (LLD)", 6 },
                    { 50, "Microservices Architecture", 6 },
                    { 51, "REST Architecture", 6 },
                    { 52, "Event-Driven Architecture", 6 },
                    { 53, "Design Patterns", 6 },
                    { 54, "Scalability & Load Balancing", 6 },
                    { 55, "Caching (Redis, Memcached)", 6 },
                    { 56, "Message Queues (Kafka, RabbitMQ)", 6 },
                    { 57, "Python", 7 },
                    { 58, "Scikit-learn", 7 },
                    { 59, "TensorFlow", 7 },
                    { 60, "PyTorch", 7 },
                    { 61, "Keras", 7 },
                    { 62, "XGBoost", 7 },
                    { 63, "LightGBM", 7 },
                    { 64, "CatBoost", 7 },
                    { 65, "TensorFlow", 8 },
                    { 66, "PyTorch", 8 },
                    { 67, "Keras", 8 },
                    { 68, "CNNs", 8 },
                    { 69, "RNNs", 8 },
                    { 70, "Transformers", 8 },
                    { 71, "OpenCV", 9 },
                    { 72, "TensorFlow", 9 },
                    { 73, "PyTorch", 9 },
                    { 74, "YOLO", 9 },
                    { 75, "MediaPipe", 9 },
                    { 76, "NLTK", 10 },
                    { 77, "SpaCy", 10 },
                    { 78, "Hugging Face Transformers", 10 },
                    { 79, "BERT", 10 },
                    { 80, "GPT Models", 10 },
                    { 81, "Python", 11 },
                    { 82, "Pandas", 11 },
                    { 83, "NumPy", 11 },
                    { 84, "SQL", 11 },
                    { 85, "Excel", 11 },
                    { 86, "Power BI", 11 },
                    { 87, "Tableau", 11 },
                    { 88, "Google Sheets", 11 },
                    { 89, "Python", 12 },
                    { 90, "R", 12 },
                    { 91, "Jupyter Notebook", 12 },
                    { 92, "Matplotlib", 12 },
                    { 93, "Seaborn", 12 },
                    { 94, "SQL", 12 },
                    { 95, "Machine Learning Libraries", 12 },
                    { 96, "Apache Spark", 13 },
                    { 97, "Apache Airflow", 13 },
                    { 98, "Hadoop", 13 },
                    { 99, "BigQuery", 13 },
                    { 100, "Snowflake", 13 },
                    { 101, "Redshift", 13 },
                    { 102, "Kafka", 13 },
                    { 103, "ETL Pipelines", 13 },
                    { 104, "Figma", 14 },
                    { 105, "Adobe XD", 14 },
                    { 106, "Sketch", 14 },
                    { 107, "InVision", 14 },
                    { 108, "Zeplin", 14 },
                    { 109, "FigJam", 14 },
                    { 110, "Figma", 15 },
                    { 111, "Adobe XD", 15 },
                    { 112, "Sketch", 15 },
                    { 113, "User Journey Mapping", 15 },
                    { 114, "Wireframing", 15 },
                    { 115, "Prototyping", 15 },
                    { 116, "Photoshop", 16 },
                    { 117, "Illustrator", 16 },
                    { 118, "InDesign", 16 },
                    { 119, "Canva", 16 },
                    { 120, "After Effects", 17 },
                    { 121, "Premiere Pro", 17 },
                    { 122, "Blender", 17 },
                    { 123, "Cinema 4D", 17 },
                    { 124, "Illustrator", 18 },
                    { 125, "Photoshop", 18 },
                    { 126, "Brand Identity Systems", 18 },
                    { 127, "Logo Design Tools", 18 },
                    { 128, "Jira", 19 },
                    { 129, "Confluence", 19 },
                    { 130, "Notion", 19 },
                    { 131, "Trello", 19 },
                    { 132, "Miro", 19 },
                    { 133, "Product Roadmaps", 19 },
                    { 134, "OKRs", 19 },
                    { 135, "Excel", 20 },
                    { 136, "Power BI", 20 },
                    { 137, "Tableau", 20 },
                    { 138, "SQL", 20 },
                    { 139, "BPMN", 20 },
                    { 140, "SWOT Analysis", 20 },
                    { 141, "Jira", 21 },
                    { 142, "Trello", 21 },
                    { 143, "Asana", 21 },
                    { 144, "Monday.com", 21 },
                    { 145, "ClickUp", 21 },
                    { 146, "Resume Review Tools", 22 },
                    { 147, "Interview Preparation Frameworks", 22 },
                    { 148, "LinkedIn Optimization", 22 },
                    { 149, "Career Planning Models", 22 },
                    { 150, "Lean Canvas", 23 },
                    { 151, "Business Model Canvas", 23 },
                    { 152, "Pitch Deck Tools", 23 },
                    { 153, "Market Research Tools", 23 },
                    { 154, "Financial Modeling", 23 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "career_goal",
                keyColumn: "career_goal_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "career_goal",
                keyColumn: "career_goal_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "career_goal",
                keyColumn: "career_goal_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "career_goal",
                keyColumn: "career_goal_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "career_goal",
                keyColumn: "career_goal_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "career_goal",
                keyColumn: "career_goal_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "AE");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "BH");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "DJ");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "DZ");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "EG");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "IQ");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "JO");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "KM");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "KW");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "LB");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "LY");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "MA");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "MR");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "OM");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "PS");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "QA");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "SA");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "SD");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "SO");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "SY");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "TN");

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "country_code",
                keyValue: "YE");

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "learning_style",
                keyColumn: "learning_style_id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "technologies",
                keyColumn: "technology_id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "subdomain",
                keyColumn: "subdomain_id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "domains",
                keyColumn: "domain_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "domains",
                keyColumn: "domain_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "domains",
                keyColumn: "domain_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "domains",
                keyColumn: "domain_id",
                keyValue: 4);
        }
    }
}
