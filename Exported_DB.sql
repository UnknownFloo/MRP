/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES  */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

CREATE TABLE IF NOT EXISTS "media" (
	"id" SERIAL NOT NULL,
	"title" VARCHAR(255) NOT NULL,
	"description" TEXT NOT NULL,
	"media_type" VARCHAR(50) NOT NULL,
	"release_year" INTEGER NOT NULL,
	"genre" TEXT NOT NULL,
	"age_restriction" INTEGER NOT NULL,
	"created_by" TEXT NULL DEFAULT NULL,
	PRIMARY KEY ("id")
);

/*!40000 ALTER TABLE "media" DISABLE KEYS */;
INSERT INTO "media" ("title", "description", "media_type", "release_year", "genre", "age_restriction", "created_by") VALUES
	('Inception', 'Sci-fi thriller', 'movie', 2010, '["sci-fi", "thriller"]', 12, 'system'),
	('The Dark Knight', 'Batman faces the Joker', 'movie', 2008, '["action", "drama"]', 15, 'system'),
	('Interstellar', 'Space exploration to save humanity', 'movie', 2014, '["sci-fi", "adventure"]', 10, 'system'),
	('The Matrix', 'A hacker discovers reality is a simulation', 'movie', 1999, '["sci-fi", "action"]', 15, 'system'),
	('Avatar', 'Humans explore Pandora', 'movie', 2009, '["sci-fi", "adventure"]', 10, 'system');
/*!40000 ALTER TABLE "media" ENABLE KEYS */;

CREATE TABLE IF NOT EXISTS "ratings" (
	"id" SERIAL NOT NULL,
	"media_id" INTEGER NOT NULL,
	"rating_by" TEXT NOT NULL,
	"stars" INTEGER NULL DEFAULT 0,
	"comment" TEXT NULL DEFAULT NULL,
	"comment_confirm" BOOLEAN NOT NULL DEFAULT false,
	"liked_by" UNKNOWN NOT NULL DEFAULT '{}',
	PRIMARY KEY ("id"),
	CONSTRAINT "ratings_stars_check" CHECK ((stars = ANY (ARRAY[1, 2, 3, 4, 5])))
);

/*!40000 ALTER TABLE "ratings" DISABLE KEYS */;
INSERT INTO "ratings" ("media_id", "rating_by", "stars", "comment", "comment_confirm", "liked_by") VALUES
	(1, 'system', 5, 'Amazing movie with a mind-bending plot.', true, '{16,18}'),
	(2, 'system', 4, 'Great action and storyline.', false, '{17}'),
	(3, 'system', 5, 'A visually stunning masterpiece.', false, '{16}'),
	(4, 'system', 5, 'A revolutionary film that changed sci-fi forever.', false, '{18}'),
	(5, 'system', 4, 'Beautiful world-building and effects.', false, '{}');

/*!40000 ALTER TABLE "ratings" ENABLE KEYS */;

CREATE TABLE IF NOT EXISTS "users" (
	"uuid" SERIAL NOT NULL,
	"username" VARCHAR(100) NOT NULL,
	"password" VARCHAR(100) NOT NULL,
	"description" TEXT NULL DEFAULT '',
	"favorites" UNKNOWN NULL DEFAULT '{}',
	PRIMARY KEY ("uuid")
);

/*!40000 ALTER TABLE "users" DISABLE KEYS */;
INSERT INTO "users" ("username", "password", "description", "favorites") VALUES
	('system', 'systempass', 'Administrator account', '{}');
/*!40000 ALTER TABLE "users" ENABLE KEYS */;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
