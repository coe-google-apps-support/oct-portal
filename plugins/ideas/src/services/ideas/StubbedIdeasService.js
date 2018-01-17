/* eslint-disable */
import { HTTP } from '../../HttpCommon'

/**
 * This acts as a stubbed out class for the ideas service.
 * Idea format:
 * idea.id
 * idea.url
 * idea.title
 * idea.description
 * idea.createdDate
 * idea.stakeholders
 * idea.stakeholders[0].userName
 */
let x = class StubbedIdeasService {

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
                            'userName': 'gregory.onuczko@edmonton.ca'
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
                {
                    id: 3,
                    url: 'http://localhost:8080',
                    title: 'Idea #3',
                    description: 'Another idea! They\'re everywhere!',
                    createdDate: 'Dec 30 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
                    stakeholders: [
                        {
                            'userName': 'daniel.chenier@edmonton.ca'
                        }
                    ]
                },
                {
                    id: 4,
                    url: 'http://localhost:8080',
                    title: 'Idea #4',
                    description: 'And another idea! They\'re everywhere!',
                    createdDate: 'Dec 31 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
                    stakeholders: [
                        {
                            'userName': 'daniel.chenier@edmonton.ca'
                        }
                    ]
                },
                {
                    id: 5,
                    url: 'http://localhost:8080',
                    title: 'I want to create an AI version of both Dan and Jared...',
                    description: `I believe we can create single Android and implant both Dan and Jared's brains into this singularity. This new life form will then be used to develop a new Blockchain methodology . I want to own 50% of the initial ICO Token's prior to launch. I want this new AI being to make me very very rich. I will then rule the world. This new fully automatism being will hack into every government military mainframe and manipulate all launch code systems and use them against the oppressive regimes in order to gain global power over all humans and make me the supreme leader! I do see this being funded by the City.`,
                    createdDate: 'Jan 16 2018 10:40:16 GMT-0700 (Mountain Standard Time)',
                    stakeholders: [
                        {
                            'userName': 'stephen.mundy@edmonton.ca'
                        }
                    ]
                },
            ]
        }
        
        return new Promise((resolve, reject) => {
            setTimeout(() => {
                resolve(fakeIdeas);
            }, 200)
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

export const StubbedIdeasService = x