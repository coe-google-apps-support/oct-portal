/* eslint-disable */
import { HTTP } from '../HttpCommon'

let x = class IdeasService {

    constructor() {
        
    }

    /**
     * Returns a Promise that resolves with a list of ideas.
     * @returns {Promise} Resolved with an array of ideas.
     */
    static getIdeas() {
        const fakeIdeas = {
            data: [
                {
                    id: 1,
                    url: 'http://localhost:8080',
                    title: 'Idea #1',
                    description: 'Easily one of the greatest ideas ever made, this idea does things for people that people really really need. Game changing. Mind blowing. World altering. Welcome to Idea #1.',
                    createdDate: 'Jan 14 2018 10:40:16 GMT-0700 (Mountain Standard Time)',
                    stakeholders: [
                        {
                            'userName': 'jared.rewerts@edmonton.ca'
                        }
                    ]
                },
                {
                    id: 2,
                    url: 'http://localhost:8080',
                    title: 'Idea #2',
                    description: 'Easily one of the lamest ideas ever made, this idea does things for people that people really really do not need. Mediocre. Milquetoast. Moronic. Welcome to Idea #2.',
                    createdDate: 'Dec 28 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
                    stakeholders: [
                        {
                            'userName': 'jared.rewerts@edmonton.ca'
                        }
                    ]
                },
            ]
        }

        return new Promise((resolve, reject) => {
            setTimeout(() => {
                resolve(fakeIdeas);
            }, 2000)
        });
    }

    /**
     * Idea format:
     * idea.id
     * idea.url
     * idea.title
     * idea.description
     * idea.createdDate
     * idea.stakeholders
     * idea.stakeholders[0].userName
     * 
     */
}

export const IdeasService = x