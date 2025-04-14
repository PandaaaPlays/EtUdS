-- Table: Etudiant
CREATE TABLE IF NOT EXISTS Etudiant (
    id SERIAL PRIMARY KEY,
    prenom VARCHAR NOT NULL,
    nom VARCHAR NOT NULL,
    courriel VARCHAR NOT NULL,
    mot_de_passe VARCHAR NOT NULL
);

-- Table: Professeur
CREATE TABLE IF NOT EXISTS Professeur (
    id SERIAL PRIMARY KEY,
    prenom VARCHAR NOT NULL,
    nom VARCHAR NOT NULL,
    courriel VARCHAR NOT NULL
);

-- Table: Cours
CREATE TABLE IF NOT EXISTS Cours (
    id SERIAL PRIMARY KEY,
    id_professeur INT REFERENCES Professeur(id),
    sigle VARCHAR NOT NULL,
    titre VARCHAR NOT NULL
);

-- Table: Enseignement
CREATE TABLE IF NOT EXISTS Enseignement (
    id SERIAL PRIMARY KEY,
    id_cours INT REFERENCES Cours(id),
    id_professeur INT REFERENCES Professeur(id)
);

-- Table: Seance
CREATE TABLE IF NOT EXISTS Seance (
    id SERIAL PRIMARY KEY,
    id_cours INT REFERENCES Cours(id),
    debut TIMESTAMP NOT NULL,
    fin TIMESTAMP NOT NULL,
    local VARCHAR NOT NULL,
    laboratoire BOOLEAN NOT NULL -- C'est un laboratoire ou non?
);

-- Table: Inscription
CREATE TABLE IF NOT EXISTS Inscription (
    id SERIAL PRIMARY KEY,
    id_etudiant INT REFERENCES Etudiant(id),
    id_cours INT REFERENCES Cours(id)
);

-- Table: Devoir
CREATE TABLE IF NOT EXISTS Devoir (
    id SERIAL PRIMARY KEY,
    id_cours INT REFERENCES Cours(id),
    nom VARCHAR NOT NULL,
    date_limite_soumission TIMESTAMP NOT NULL,
    taille_max_equipes INT NOT NULL
);

-- Table: Equipe
CREATE TABLE IF NOT EXISTS Equipe (
    id SERIAL PRIMARY KEY,
    id_devoir INT REFERENCES Devoir(id),
    etat_soumission INT NOT NULL, -- À faire = 1
                                  -- En retard = 2
                                  -- Soumis = 3
                                  -- Evalué = 4
    note NUMERIC(5, 2) -- Nullable (avant la correction)
);

-- Table: Membre
CREATE TABLE IF NOT EXISTS Membre (
    id SERIAL PRIMARY KEY,
    id_etudiant INT REFERENCES Etudiant(id),
    id_equipe INT REFERENCES Equipe(id),
    accepte BOOLEAN NOT NULL  -- Est accepté ou en invitation?                     
);

-- Table: Document
CREATE TABLE IF NOT EXISTS Document (
    id SERIAL PRIMARY KEY,
    id_equipe INT REFERENCES Equipe(id),
    id_membre INT REFERENCES Membre(id),
    taille BIGINT NOT NULL,
    nom_fichier VARCHAR NOT NULL
);

-- Table: Soumission
CREATE TABLE IF NOT EXISTS Soumission (
    id SERIAL PRIMARY KEY,
    id_equipe INT REFERENCES Equipe(id),
    temps_soumission TIMESTAMP NOT NULL,
    taille BIGINT NOT NULL,
    nom_fichier VARCHAR NOT NULL,
    nom_fichier_correction VARCHAR -- Null tant que la correction n'est pas donnée
);

-- Table: Note_de_Cours
CREATE TABLE IF NOT EXISTS Note_de_Cours (
    id SERIAL PRIMARY KEY,
    id_cours INT NOT NULL REFERENCES Cours(id) ON DELETE CASCADE,
    date TIMESTAMP NOT NULL DEFAULT NOW(),
    taille BIGINT NOT NULL,
    nom_fichier VARCHAR(255) NOT NULL
);



-------------------
-- Conversations -- 
------------------- 

-- Table: Conversation
CREATE TABLE Conversation (
    id SERIAL PRIMARY KEY,
    titre VARCHAR NOT NULL
);

-- Table: Conversation_Participant
CREATE TABLE Conversation_Participant (
    id SERIAL PRIMARY KEY,
    id_conversation INTEGER REFERENCES conversation(id),
    id_etudiant INTEGER REFERENCES etudiant(id),
    UNIQUE(id_conversation, id_etudiant) 
);

-- Table: Message
CREATE TABLE Message (
    id SERIAL PRIMARY KEY,
    id_conversation INTEGER REFERENCES Conversation(id),
    id_etudiant INTEGER REFERENCES etudiant(id) ON DELETE SET NULL,
    contenu VARCHAR NOT NULL,
    date_envoi TIMESTAMP NOT NULL
);

-- Table: Message_Non_Lu (Existe tant que le message n'est pas lu)
CREATE TABLE Message_Non_Lu (
    id SERIAL PRIMARY KEY,
    id_message INTEGER REFERENCES message(id),
    id_etudiant INTEGER REFERENCES etudiant(id),
    rappel_envoye BOOLEAN NOT NULL,
    UNIQUE(id_message, id_etudiant) 
);

