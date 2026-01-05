-- PostgreSQL initialization script for MRP Database

-- Create tables
CREATE TABLE IF NOT EXISTS media (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    media_type VARCHAR(50) NOT NULL,
    release_year INTEGER NOT NULL,
    genre JSONB NOT NULL,
    age_restriction INTEGER NOT NULL,
    created_by TEXT DEFAULT NULL
);

CREATE TABLE IF NOT EXISTS ratings (
    id SERIAL PRIMARY KEY,
    media_id INTEGER NOT NULL,
    rating_by TEXT NOT NULL,
    stars INTEGER DEFAULT 0,
    comment TEXT DEFAULT NULL,
    comment_confirm BOOLEAN NOT NULL DEFAULT false,
    liked_by INTEGER[] DEFAULT '{}',
    CONSTRAINT ratings_stars_check CHECK (stars IN (1, 2, 3, 4, 5)),
    CONSTRAINT fk_ratings_media FOREIGN KEY (media_id) REFERENCES media(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS users (
    uuid SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(100) NOT NULL,
    description TEXT DEFAULT '',
    favorites INTEGER[] DEFAULT '{}'
);

-- Insert sample data
INSERT INTO media (title, description, media_type, release_year, genre, age_restriction, created_by) VALUES
    ('Inception', 'Sci-fi thriller', 'movie', 2010, '["sci-fi", "thriller"]', 12, 'system'),
    ('The Dark Knight', 'Batman faces the Joker', 'movie', 2008, '["action", "drama"]', 15, 'system'),
    ('Interstellar', 'Space exploration to save humanity', 'movie', 2014, '["sci-fi", "adventure"]', 10, 'system'),
    ('The Matrix', 'A hacker discovers reality is a simulation', 'movie', 1999, '["sci-fi", "action"]', 15, 'system'),
    ('Avatar', 'Humans explore Pandora', 'movie', 2009, '["sci-fi", "adventure"]', 10, 'system');

INSERT INTO ratings (media_id, rating_by, stars, comment, comment_confirm, liked_by) VALUES
    (1, 'system', 5, 'Amazing movie with a mind-bending plot.', true, '{16,18}'),
    (2, 'system', 4, 'Great action and storyline.', false, '{17}'),
    (3, 'system', 5, 'A visually stunning masterpiece.', false, '{16}'),
    (4, 'system', 5, 'A revolutionary film that changed sci-fi forever.', false, '{18}'),
    (5, 'system', 4, 'Beautiful world-building and effects.', false, '{}');

INSERT INTO users (username, password, description, favorites) VALUES
    ('system', 'systempass', 'Administrator account', '{}'),
    ('admin', 'admin123', 'Admin user for testing', '{}');

-- Create indexes for better performance
CREATE INDEX idx_media_title ON media(title);
CREATE INDEX idx_media_genre ON media USING GIN(genre);
CREATE INDEX idx_ratings_media_id ON ratings(media_id);
CREATE INDEX idx_ratings_rating_by ON ratings(rating_by);
CREATE INDEX idx_users_username ON users(username);