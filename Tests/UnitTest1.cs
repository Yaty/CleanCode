using System;
using Xunit;
using LightSaberFactory;

namespace Tests
{
    public class UnitTest1 {
        [Fact]
        public void it_should_be_a_valid_config(){
            TemplateParser parser = new TemplateParser();
            string[] lines = { "template:register-confirmation", "name:Yoda", "title:Master", "mail:yoda@dagobah.com", "code:Y456"};
            TemplateConfig templateConfig = parser.ParseConfig(lines);
            Assert.True(templateConfig.IsValid());
        }

        [Fact]
        public void it_should_detect_invalid_config(){
            TemplateParser parser = new TemplateParser();
            string[] lines = { "template:register", "name:Yoda", "title:Master", "mail:yoda@dagobah.com", "code:Y456"};
            TemplateConfig templateConfig = parser.ParseConfig(lines);
            Assert.False(templateConfig.IsValid());
        }

        [Fact]
        public void it_should_detect_invalid_config_with_missing_element(){
            TemplateParser parser = new TemplateParser();
            string[] lines = { "template:register-confirmation", "name:Yoda", "title:Master", "code:Y456"};
            TemplateConfig templateConfig = parser.ParseConfig(lines);
            Assert.False(templateConfig.IsValid());
        }

        [Fact]
        public void it_should_fill_a_template(){
            TemplateParser parser = new TemplateParser();
            string[] lines = { "template:register-confirmation", "name:Yoda", "title:Master", "mail:yoda@dagobah.com", "code:Y456"};
            TemplateConfig templateConfig = parser.ParseConfig(lines);

            string template = @"# Bonjour {{title}} {{name}} !

Merci de vous être inscrit sur le site [{{=website}}]({{=website}}), vous allez
pouvoir commander bientôt la toute dernière technologie de sabres laser.

Vous devez pour cela valider votre mail {{mail}} en cliquand sur le lien suivant:

	[Validation]({{=website}}/activation/{{code}})


Bonne journée!

La guilde des Sabres Laser.

(mail généré a {{=datetime}})";

            
            string content = parser.ParseContent(template, templateConfig);

            Assert.Equal(content, @"# Bonjour Master Yoda !

Merci de vous être inscrit sur le site [http://thelightsabersguild.com](http://thelightsabersguild.com), vous allez
pouvoir commander bientôt la toute dernière technologie de sabres laser.

Vous devez pour cela valider votre mail yoda@dagobah.com en cliquand sur le lien suivant:

	[Validation](http://thelightsabersguild.com/activation/Y456)


Bonne journée!

La guilde des Sabres Laser.

(mail généré a " + DateTime.Now.ToString() + ")");
        }
    }
}
